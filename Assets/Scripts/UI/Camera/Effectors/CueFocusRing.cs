using UnityEngine;

[AddComponentMenu("UI/Camera/Effectors/Cue Focus Ring")]
[RequireComponent(typeof(CircleCollider2D))]
public sealed class CueFocusRing : CueFocusBase
{
	[Tooltip("when true, an additional inner ring can be used to have it's own specific weight indpendent of the outer ring")]
	[SerializeField]
	private bool enableInnerRing = false;
	[SerializeField]
	private float innerRingRadius = 2f;
	[SerializeField]
	private float innerEffectorWeight = 5f;
	[SerializeField]
	private bool enableEffectorFalloff = true;
	[Tooltip("The curve should go from 0 to 1 being the normalized distance from center to radius. It's value will be multiplied by the effectorWeight to get the final weight used.")]
	[SerializeField]
	private AnimationCurve effectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private new CircleCollider2D EffectorTrigger => base.EffectorTrigger as CircleCollider2D;

	private Vector3 EffectorPosition
	{
		get { return transform.TransformPoint(EffectorTrigger.center); }
		set { EffectorTrigger.center = transform.InverseTransformPoint(value); }
	}

	private float OuterRingRadius => EffectorTrigger.radius;

	#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		UnityEditor.Handles.color = Color.green;

		if (EffectorTrigger != null)
		{
			if (enableInnerRing)
				UnityEditor.Handles.DrawWireDisc(EffectorPosition, Vector3.back, innerRingRadius);

			UnityEditor.Handles.PositionHandle(EffectorPosition, Quaternion.identity);
		}
	}
	#endif

	public override float GetEffectorWeight()
	{		
		var distanceToEffector = Vector3.Distance(EffectorPosition, trackedTarget.position);

		if (enableInnerRing && distanceToEffector <= innerRingRadius)
			return innerEffectorWeight;

		if (enableEffectorFalloff)
			return effectorFalloff.Evaluate(1f - (distanceToEffector / OuterRingRadius)) * effectorWeight;
		else
			return effectorWeight;
	}
}
