using System.Diagnostics;
using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  [RequireComponent(typeof(Collider2D))]
  public abstract class CueFocusBase : MonoBehaviour, ICameraEffector
  {
    [SerializeField] private Collider2D effectorTrigger = null;

    [SerializeField, BitMask] private CameraAxis axis = CameraAxis.Horizontal;
    [SerializeField] private float effectorWeight = 0.5f;
    [Tooltip("when true, an additional inner ring can be used to have it's own specific weight indpendent of the outer ring")] [SerializeField] protected bool onlyTriggerWhenGrounded = false;

    [Inject]
    private CameraController CameraController { get; set; }

    [Inject(Tags.Player)]
    private IMovable PlayerMovement { get; set; }

    protected Transform TrackedTarget { get; private set; }

    protected float EffectorWeight => this.effectorWeight;
    protected Collider2D EffectorTrigger => this.effectorTrigger;
    protected virtual Vector3 FocusPosition => transform.position;
    protected bool AffectHorizontal => (this.axis & CameraAxis.Horizontal) == CameraAxis.Horizontal;
    protected bool AffectVertical => (this.axis & CameraAxis.Vertical) == CameraAxis.Vertical;

    [Conditional("UNITY_EDITOR")]
    protected void Update() { }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
      if (TrackedTarget == null
          && other.tag == Tags.Player
          && (!this.onlyTriggerWhenGrounded || PlayerMovement.IsGrounded))
        Activate(other.transform);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
      => OnTriggerEnter2D(other);

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
      if (TrackedTarget != null && other.tag == Tags.Player)
        Deactivate();
    }

    protected virtual void OnDisable()
      => Deactivate();

    public virtual Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
    {
      var targetPosition = basePosition;

      if (AffectHorizontal)
        targetPosition.x = FocusPosition.x;
      if (AffectVertical)
        targetPosition.y = FocusPosition.y;

      return targetPosition;
    }

    public abstract float GetEffectorWeight();

    protected virtual void Activate(Transform newTransform)
    {
      TrackedTarget = newTransform;
      CameraController.AddCameraEffector(this);
    }

    protected virtual void Deactivate()
    {
      TrackedTarget = null;
      CameraController.RemoveCameraEffector(this);
    }
  }
}
