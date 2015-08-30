using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	[NonSerialized]
	[HideInInspector]
	public new Camera camera;
	public Collider2D targetCollider;
	[Tooltip("percentage from -0.5 - 0.5 from the center of the screen")]
	[Range(-0.5f, 0.5f)]
	public float horizontalOffset = 0f;
	[Tooltip("percentage from -0.5 - 0.5 from the center of the screen")]
	[Range(-0.5f, 0.5f)]
	public float verticalOffset = 0f;

	[Header("Platform Snap")]
	[Tooltip("all platform snap settings only apply if enablePlatformSnap is true")]
	public bool enablePlatformSnap;
	[Tooltip("If true, no other base behaviors will be able to modify the y-position of the camera when grounded")]
	public bool isPlatformSnapExclusiveWhenEnabled;
	[Range(-10f, 10f)]
	public float platformSnapVerticalOffset = 0f;

	[Header("Smoothing")]
	public CameraSmoothingType cameraSmoothingType;
	[Tooltip("approximately the time it will take to reach the target. A smaller value will reach the target faster.")]
	public float smoothDampTime = 0.08f;
	[Tooltip("lower values are less damped and higher values are more damped resulting in less springiness. should be between 0.01f, 1f to avoid unstable systems.")]
	public float springDampingRatio = 0.7f;
	[Tooltip("An angular frequency of 2pi (radians per second) means the oscillation completes one full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable")]
	public float springAngularFrequency = 20f;
	public float lerpTowardsFactor = 0.002f;

	private Transform _transform;
	private List<ICameraBaseBehavior> baseCameraBehaviors = new List<ICameraBaseBehavior>(3);
	private List<ICameraEffector> cameraEffectors = new List<ICameraEffector>(3);
	private List<ICameraFinalizer> cameraFinalizers = new List<ICameraFinalizer>(1);

	private FixedSizedVector3Queue averageVelocityQueue = new FixedSizedVector3Queue(10);
	private Vector3 targetPositionLastFrame;
	private Vector3 cameraVelocity;

	private static CameraController instance;

	public static CameraController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType(typeof(CameraController)) as CameraController;

				if (instance == null) throw new UnityException("CameraController does not appear to exist");
			}

			return instance;
		}
	}

	#region MonoBehaviour
	private void Awake()
	{
		instance = this;
		_transform = GetComponent<Transform>();
		camera = GetComponent<Camera>();

		foreach (var baseBehavior in this.GetInterfaceComponents<ICameraBaseBehavior>())
			AddCameraBaseBehavior(baseBehavior);

		foreach (var finalizer in this.GetInterfaceComponents<ICameraFinalizer>())
			AddCameraFinalizer(finalizer);
	}

	private void LateUpdate()
	{
		var targetBounds = targetCollider.bounds;

		// we keep track of the target's velocity since some camera behaviors need to know about it
		var velocity = (targetBounds.center - targetPositionLastFrame) / Time.deltaTime;
		velocity.z = 0f;
		averageVelocityQueue.Push(velocity);
		targetPositionLastFrame = targetBounds.center;

		// fetch the average velocity for use in our camera behaviors
		var targetAvgVelocity = averageVelocityQueue.Average();

		// we use the transform.position plus the offset when passing the base position to our camera behaviors
		var basePosition = GetNormalizedCameraPosition();
		var accumulatedDeltaOffset = Vector3.zero;

		for (var i = 0; i < baseCameraBehaviors.Count; i++)
		{
			var cameraBehavior = baseCameraBehaviors[i];

			if (cameraBehavior.IsEnabled())
			{
				// once we get the desired position we have to subtract the offset that we previously added
				var desiredPos = cameraBehavior.GetDesiredPositionDelta(targetBounds, basePosition, targetAvgVelocity);
				accumulatedDeltaOffset += desiredPos;
			}
		}

		if (enablePlatformSnap && PlayerControl.Instance.IsGrounded)
		{
			// when exclusive, no base behaviors can mess with y
			if (isPlatformSnapExclusiveWhenEnabled)
				accumulatedDeltaOffset.y = 0f;

			var desiredOffset = targetBounds.min.y - basePosition.y - platformSnapVerticalOffset;
			accumulatedDeltaOffset += new Vector3(0f, desiredOffset);
		}

		// fetch our effectors
		var totalWeight = 0f;
		var accumulatedEffectorPosition = Vector3.zero;

		for (var i = 0; i < cameraEffectors.Count; i++)
		{
			var weight = cameraEffectors[i].GetEffectorWeight();
			var position = cameraEffectors[i].GetDesiredPositionDelta(targetBounds, basePosition, targetAvgVelocity);

			totalWeight += weight;
			accumulatedEffectorPosition += (weight * position);
		}

		var desiredPosition = _transform.position + accumulatedDeltaOffset;

		// if we have a totalWeight we need to take into account our effectors
		if (totalWeight > 0)
		{
			totalWeight += 1f;
			accumulatedEffectorPosition += desiredPosition;

			var finalAccumulatedPosition = accumulatedEffectorPosition / totalWeight;

			finalAccumulatedPosition.z = _transform.position.z;
			desiredPosition = finalAccumulatedPosition;
		}

		var smoothing = cameraSmoothingType;

		// and finally, our finalizers have a go if we have any
		for (var i = 0; i < cameraFinalizers.Count; i++)
		{
			desiredPosition = cameraFinalizers[i].GetFinalCameraPosition(targetBounds, transform.position, desiredPosition);

			// allow the finalizer with a 0 priority to skip smoothing if it wants to
			if (i == 0 && cameraFinalizers[i].GetFinalizerPriority() == 0 && cameraFinalizers[i].ShouldSkipSmoothingThisFrame())
				smoothing = CameraSmoothingType.None;
		}

		// reset Z just in case one of the other scripts messed with it
		desiredPosition.z = _transform.position.z;

		// time to smooth our movement to the desired position
		switch (smoothing)
		{
			case CameraSmoothingType.None:
				_transform.position = desiredPosition;
				break;
			case CameraSmoothingType.SmoothDamp:
				_transform.position = Vector3.SmoothDamp(_transform.position, desiredPosition, ref cameraVelocity, smoothDampTime);
				break;
			case CameraSmoothingType.Spring:
				_transform.position = FastSpring(_transform.position, desiredPosition);
				break;
			case CameraSmoothingType.Lerp:
				_transform.position = LerpTowards(_transform.position, desiredPosition, lerpTowardsFactor);
				break;
		}
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var positionInFrontOfCamera = GetNormalizedCameraPosition();

		positionInFrontOfCamera.z = 1f;

		foreach (var baseBehavior in this.GetInterfaceComponents<ICameraBaseBehavior>())
			if (baseBehavior.IsEnabled())
				baseBehavior.onDrawGizmos(positionInFrontOfCamera);

		foreach (var finalizer in this.GetInterfaceComponents<ICameraFinalizer>())
			if (finalizer.IsEnabled())
				finalizer.onDrawGizmos(positionInFrontOfCamera);

		if (enablePlatformSnap)
		{
			Gizmos.color = new Color(0.3f, 0.1f, 0.6f);

			var lineWidth = Camera.main.orthographicSize / 2f;

			Gizmos.DrawLine(positionInFrontOfCamera + new Vector3(-lineWidth, platformSnapVerticalOffset, 1f), 
											positionInFrontOfCamera + new Vector3(lineWidth, platformSnapVerticalOffset, 1f));
		}
	}
	#endif

	private void OnApplicationQuit()
	{
		instance = null;
	}
	#endregion

	private Vector3 GetNormalizedCameraPosition()
	{
		//Camera.main.ViewportToWorldPoint()
		#if UNITY_EDITOR
		return GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f + horizontalOffset, 0.5f + verticalOffset, 0f));
		#else
		return camera.ViewportToWorldPoint( new Vector3( 0.5f + horizontalOffset, 0.5f + verticalOffset, 0f ) );
		#endif
	}

	#region smoothing
	private Vector3 LerpTowards(Vector3 from, Vector3 to, float remainingFactorPerSecond)
	{
		return Vector3.Lerp(from, to, 1f - Mathf.Pow(remainingFactorPerSecond, Time.deltaTime));
	}

	/// <summary>
	/// uses the semi-implicit euler method. faster, but not always stable.
	/// see http://allenchou.net/2015/04/game-math-more-on-numeric-springing/
	/// </summary>
	/// <returns>The spring.</returns>
	/// <param name="currentValue">Current value.</param>
	/// <param name="targetValue">Target value.</param>
	/// <param name="velocity">Velocity by reference. Be sure to reset it to 0 if changing the targetValue between calls</param>
	/// <param name="dampingRatio">lower values are less damped and higher values are more damped resulting in less springiness.
	/// should be between 0.01f, 1f to avoid unstable systems.</param>
	/// <param name="angularFrequency">An angular frequency of 2pi (radians per second) means the oscillation completes one
	/// full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable</param>
	private Vector3 FastSpring(Vector3 currentValue, Vector3 targetValue)
	{
		cameraVelocity += -2.0f * Time.deltaTime * springDampingRatio * springAngularFrequency * cameraVelocity + Time.deltaTime * springAngularFrequency * springAngularFrequency * (targetValue - currentValue);
		currentValue += Time.deltaTime * cameraVelocity;

		return currentValue;
	}
	#endregion


	#region Camera Behavior and Effector management
	public void AddCameraBaseBehavior(ICameraBaseBehavior cameraBehavior)
	{
		baseCameraBehaviors.Add(cameraBehavior);
	}

	public void RemoveCameraBaseBehavior<T>() where T : ICameraBaseBehavior
	{
		var requestedType = typeof(T);

		for (var i = baseCameraBehaviors.Count - 1; i >= 0; i--)
		{
			if (baseCameraBehaviors[i].GetType() == requestedType)
			{
				baseCameraBehaviors.RemoveAt(i);
				return;
			}
		}
	}

	public T GetCameraBaseBehavior<T>() where T : ICameraBaseBehavior
	{
		var requestedType = typeof(T);

		for (var i = 0; i < baseCameraBehaviors.Count; i++)
		{
			if (baseCameraBehaviors[i].GetType() == requestedType)
				return (T)baseCameraBehaviors[i];
		}

		return default(T);
	}

	public void AddCameraEffector(ICameraEffector cameraEffector)
	{
		cameraEffectors.Add(cameraEffector);
	}

	public void RemoveCameraEffector(ICameraEffector cameraEffector)
	{
		for (var i = cameraEffectors.Count - 1; i >= 0; i--)
		{
			if (cameraEffectors[i] == cameraEffector)
			{
				cameraEffectors.RemoveAt(i);
				return;
			}
		}
	}

	public void AddCameraFinalizer(ICameraFinalizer cameraFinalizer)
	{
		cameraFinalizers.Add(cameraFinalizer);

		// sort the list if we need to
		if (cameraFinalizers.Count > 1)
			cameraFinalizers.Sort((first, second) => first.GetFinalizerPriority().CompareTo(second.GetFinalizerPriority()));
	}

	public void RemoveCameraFinalizer(ICameraFinalizer cameraFinalizer)
	{
		for (var i = cameraFinalizers.Count - 1; i >= 0; i--)
		{
			if (cameraFinalizers[i] == cameraFinalizer)
			{
				cameraFinalizers.RemoveAt(i);
				return;
			}
		}
	}
	#endregion

}