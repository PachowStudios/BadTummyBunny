using UnityEngine;

public class CueFocusRing : MonoBehaviour, ICameraEffector
{
	public CircleCollider2D effectorTrigger;
	[BitMask]
	public CameraAxis axis;
	public float effectorWeight = 0.5f;
	[Tooltip("when true, an additional inner ring can be used to have it's own specific weight indpendent of the outer ring")]
	public bool enableInnerRing = false;
	public float innerRingRadius = 2f;
	public float innerEffectorWeight = 5f;
	public bool enableEffectorFalloff = true;
	[Tooltip("The curve should go from 0 to 1 being the normalized distance from center to radius. It's value will be multiplied by the effectorWeight to get the final weight used.")]
	public AnimationCurve effectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	public bool onlyTriggerWhenGrounded = false;

	private Transform trackedTarget;

	private Vector3 EffectorPosition
	{ get { return effectorTrigger.transform.position; } }

	private float OuterRingRadius
	{ get { return effectorTrigger.radius; } }

	private bool EffectHorizontal
	{ get { return (axis & CameraAxis.Horizontal) == CameraAxis.Horizontal; } }

	private bool EffectVertical
	{ get { return (axis & CameraAxis.Vertical) == CameraAxis.Vertical; } }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == Tags.Player)
		{
			if (!onlyTriggerWhenGrounded || PlayerControl.Instance.IsGrounded)
			{
				trackedTarget = other.transform;
				CameraController.Instance.AddCameraEffector(this);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (trackedTarget == null && other.tag == Tags.Player)
			OnTriggerEnter2D(other);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.transform == trackedTarget)
		{
			trackedTarget = null;
			CameraController.Instance.RemoveCameraEffector(this);
		}
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		UnityEditor.Handles.color = Color.green;

		if (enableInnerRing) UnityEditor.Handles.DrawWireDisc(EffectorPosition, Vector3.back, innerRingRadius);
	}
	#endif

	#region ICameraEffector
	public float GetEffectorWeight()
	{		
		var distanceToEffector = Vector3.Distance(EffectorPosition, trackedTarget.position);

		if (enableInnerRing && distanceToEffector <= innerRingRadius)
			return innerEffectorWeight;

		if (enableEffectorFalloff)
			return effectorFalloff.Evaluate(1f - (distanceToEffector / OuterRingRadius)) * effectorWeight;
		else
			return effectorWeight;
	}

	public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAvgVelocity)
	{
		var targetPosition = basePosition;

		if (EffectHorizontal) targetPosition.x = transform.position.x;
		if (EffectVertical) targetPosition.y = transform.position.y;

		return targetPosition;
	}
	#endregion
}
