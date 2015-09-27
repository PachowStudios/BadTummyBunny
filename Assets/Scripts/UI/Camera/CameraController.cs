using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("UI/Camera/Camera Controller")]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
  public Collider2D targetCollider;
  [Tooltip("percentage from -0.5 - 0.5 from the center of the screen")]
  [SerializeField, Range(-0.5f, 0.5f)] private float horizontalOffset = 0f;
  [Tooltip("percentage from -0.5 - 0.5 from the center of the screen")]
  [SerializeField, Range(-0.5f, 0.5f)] private float verticalOffset = 0f;

  [Header("Platform Snap")]
  [Tooltip("all platform snap settings only apply if enablePlatformSnap is true")]
  [SerializeField] private bool enablePlatformSnap = false;
  [Tooltip("If true, no other base behaviors will be able to modify the y-position of the camera when grounded")]
  [SerializeField] private bool isPlatformSnapExclusiveWhenEnabled = false;
  [SerializeField, Range(-10f, 10f)] private float platformSnapVerticalOffset = 0f;

  [Header("Smoothing")]
  [SerializeField] private CameraSmoothingType cameraSmoothingType = CameraSmoothingType.SmoothDamp;
  [Tooltip("approximately the time it will take to reach the target. A smaller value will reach the target faster.")]
  [SerializeField] private float smoothDampTime = 0.08f;
  [Tooltip("lower values are less damped and higher values are more damped resulting in less springiness. should be between 0.01f, 1f to avoid unstable systems.")]
  [SerializeField] private float springDampingRatio = 0.7f;
  [Tooltip("An angular frequency of 2pi (radians per second) means the oscillation completes one full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable")]
  [SerializeField] private float springAngularFrequency = 20f;
  [SerializeField] private float lerpTowardsFactor = 0.002f;

  private List<ICameraBaseBehavior> baseCameraBehaviors = new List<ICameraBaseBehavior>(3);
  private List<ICameraEffector> cameraEffectors = new List<ICameraEffector>(3);
  private List<ICameraFinalizer> cameraFinalizers = new List<ICameraFinalizer>(1);

  private FixedSizedVector3Queue averageVelocityQueue = new FixedSizedVector3Queue(10);
  private Vector3 targetPositionLastFrame;
  private Vector3 cameraVelocity;

  private Camera controlledCamera;

  private static CameraController instance;

  public static CameraController Instance => instance ?? (CameraController)FindObjectOfType(typeof(CameraController));

  public Camera Camera => this.GetComponentIfNull(ref this.controlledCamera);

  private void Awake()
  {
    instance = this;

    foreach (var baseBehavior in this.GetInterfaces<ICameraBaseBehavior>())
      AddCameraBaseBehavior(baseBehavior);

    foreach (var finalizer in this.GetInterfaces<ICameraFinalizer>())
      AddCameraFinalizer(finalizer);
  }

  private void LateUpdate()
  {
    var targetBounds = this.targetCollider.bounds;

    // we keep track of the target's velocity since some camera behaviors need to know about it
    var velocity = (targetBounds.center - this.targetPositionLastFrame) / Time.deltaTime;
    velocity.z = 0f;
    this.averageVelocityQueue.Push(velocity);
    this.targetPositionLastFrame = targetBounds.center;

    // fetch the average velocity for use in our camera behaviors
    var targetAvgVelocity = this.averageVelocityQueue.Average();

    // we use the transform.position plus the offset when passing the base position to our camera behaviors
    var basePosition = GetNormalizedCameraPosition();
    var accumulatedDeltaOffset = this.baseCameraBehaviors.Where(cameraBehavior => cameraBehavior.IsEnabled)
                                                         .Select(cameraBehavior => cameraBehavior.GetDesiredPositionDelta(targetBounds, basePosition, targetAvgVelocity))
                                                         .Aggregate(Vector3.zero, (current, desiredPos) => current + desiredPos);

    if (this.enablePlatformSnap && Player.Instance.Movement.IsGrounded)
    {
      // when exclusive, no base behaviors can mess with y
      if (this.isPlatformSnapExclusiveWhenEnabled)
        accumulatedDeltaOffset.y = 0f;

      var desiredOffset = targetBounds.min.y - basePosition.y - this.platformSnapVerticalOffset;

      accumulatedDeltaOffset += new Vector3(0f, desiredOffset);
    }

    // fetch our effectors
    var totalWeight = 0f;
    var accumulatedEffectorPosition = Vector3.zero;

    foreach (var effector in this.cameraEffectors)
    {
      var weight = effector.GetEffectorWeight();
      var position = effector.GetDesiredPositionDelta(targetBounds, basePosition, targetAvgVelocity);

      totalWeight += weight;
      accumulatedEffectorPosition += (weight * position);
    }

    var desiredPosition = transform.position + accumulatedDeltaOffset;

    // if we have a totalWeight we need to take into account our effectors
    if (totalWeight > 0)
    {
      totalWeight += 1f;
      accumulatedEffectorPosition += desiredPosition;

      var finalAccumulatedPosition = accumulatedEffectorPosition / totalWeight;

      finalAccumulatedPosition.z = transform.position.z;
      desiredPosition = finalAccumulatedPosition;
    }

    var smoothing = this.cameraSmoothingType;

    // and finally, our finalizers have a go if we have any
    for (var i = 0; i < this.cameraFinalizers.Count; i++)
    {
      desiredPosition = this.cameraFinalizers[i].GetFinalCameraPosition(targetBounds, transform.position, desiredPosition);

      // allow the finalizer with a 0 priority to skip smoothing if it wants to
      if (i == 0 && this.cameraFinalizers[i].GetFinalizerPriority == 0 && this.cameraFinalizers[i].ShouldSkipSmoothingThisFrame)
        smoothing = CameraSmoothingType.None;
    }

    // reset Z just in case one of the other scripts messed with it
    desiredPosition.z = transform.position.z;

    // time to smooth our movement to the desired position
    switch (smoothing)
    {
      case CameraSmoothingType.None:
        transform.position = desiredPosition;
        break;
      case CameraSmoothingType.SmoothDamp:
        transform.position = Vector3.SmoothDamp(transform.position, 
                                                desiredPosition, 
                                                ref this.cameraVelocity, 
                                                this.smoothDampTime);
        break;
      case CameraSmoothingType.Spring:
        transform.position = FastSpring(transform.position, desiredPosition);
        break;
      case CameraSmoothingType.Lerp:
        transform.position = LerpTowards(transform.position, desiredPosition, this.lerpTowardsFactor);
        break;
    }
  }

  #if UNITY_EDITOR
  private void OnDrawGizmos()
  {
    var positionInFrontOfCamera = GetNormalizedCameraPosition();

    positionInFrontOfCamera.z = 1f;

    foreach (var baseBehavior in this.GetInterfaces<ICameraBaseBehavior>()
                                     .Where(baseBehavior => baseBehavior.IsEnabled))
      baseBehavior.onDrawGizmos(positionInFrontOfCamera);

    foreach (var finalizer in this.GetInterfaces<ICameraFinalizer>()
                                  .Where(finalizer => finalizer.IsEnabled))
      finalizer.onDrawGizmos(positionInFrontOfCamera);

    if (!this.enablePlatformSnap)
      return;

    Gizmos.color = new Color(0.3f, 0.1f, 0.6f);

    var lineWidth = Camera.main.orthographicSize / 2f;

    Gizmos.DrawLine(positionInFrontOfCamera + new Vector3(-lineWidth, this.platformSnapVerticalOffset, 1f),
                    positionInFrontOfCamera + new Vector3(lineWidth, this.platformSnapVerticalOffset, 1f));
  }
  #endif

  private void OnApplicationQuit() 
    => instance = null;
  
  private Vector3 GetNormalizedCameraPosition() 
  => Camera.ViewportToWorldPoint(new Vector3(0.5f + this.horizontalOffset, 0.5f + this.verticalOffset, 0f));

  private Vector3 LerpTowards(Vector3 from, Vector3 to, float remainingFactorPerSecond) 
    => Vector3.Lerp(from, to, 1f - Mathf.Pow(remainingFactorPerSecond, Time.deltaTime));

  private Vector3 FastSpring(Vector3 currentValue, Vector3 targetValue)
  {
    this.cameraVelocity += (-2.0f * Time.deltaTime * this.springDampingRatio * this.springAngularFrequency * this.cameraVelocity) 
                        +  (Time.deltaTime * this.springAngularFrequency * this.springAngularFrequency * (targetValue - currentValue));
    currentValue += Time.deltaTime * this.cameraVelocity;

    return currentValue;
  }

  public void AddCameraBaseBehavior(ICameraBaseBehavior cameraBehavior) 
    => this.baseCameraBehaviors.Add(cameraBehavior);

  public void RemoveCameraBaseBehavior<T>()
    where T : ICameraBaseBehavior
      => this.baseCameraBehaviors.RemoveAll(b => b is T);

  public T GetCameraBaseBehavior<T>()
    where T : ICameraBaseBehavior
      => (T)this.baseCameraBehaviors.FirstOrDefault(b => b is T);

  public void AddCameraEffector(ICameraEffector cameraEffector)
    => this.cameraEffectors.Add(cameraEffector);

  public void RemoveCameraEffector(ICameraEffector cameraEffector)
    => this.cameraEffectors.RemoveAll(e => e == cameraEffector);

  public void AddCameraFinalizer(ICameraFinalizer cameraFinalizer)
  {
    this.cameraFinalizers.Add(cameraFinalizer);

    if (this.cameraFinalizers.Count > 1)
      this.cameraFinalizers.Sort((first, second) => first.GetFinalizerPriority.CompareTo(second.GetFinalizerPriority));
  }

  public void RemoveCameraFinalizer(ICameraFinalizer cameraFinalizer)
    => this.cameraFinalizers.RemoveAll(f => f == cameraFinalizer);
}
