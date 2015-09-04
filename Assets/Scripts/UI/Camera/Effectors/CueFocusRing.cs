using UnityEngine;

[AddComponentMenu("UI/Camera/Effectors/Cue Focus Ring")]
[RequireComponent(typeof(CircleCollider2D))]
public class CueFocusRing : CueFocusBase
{
	[Tooltip("when true, an additional inner ring can be used to have it's own specific weight indpendent of the outer ring")]
	public bool enableInnerRing = false;
	public float innerRingRadius = 2f;
	public float innerEffectorWeight = 5f;
	public bool enableEffectorFalloff = true;
	[Tooltip("The curve should go from 0 to 1 being the normalized distance from center to radius. It's value will be multiplied by the effectorWeight to get the final weight used.")]
	public AnimationCurve effectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	protected new CircleCollider2D EffectorTrigger
	{ get { return base.EffectorTrigger as CircleCollider2D; } }

	private Vector3 EffectorPosition
	{
		get { return transform.TransformPoint(EffectorTrigger.center); }
		set { EffectorTrigger.center = transform.InverseTransformPoint(value); }
	}

	private float OuterRingRadius
	{ get { return EffectorTrigger.radius; } }

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
