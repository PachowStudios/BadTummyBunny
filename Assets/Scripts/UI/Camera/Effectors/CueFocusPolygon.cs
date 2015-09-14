using UnityEngine;

[AddComponentMenu("UI/Camera/Effectors/Cue Focus Polygon")]
[RequireComponent(typeof(PolygonCollider2D))]
public class CueFocusPolygon : CueFocusBase
{
	protected new PolygonCollider2D EffectorTrigger => base.EffectorTrigger as PolygonCollider2D;

	public override float GetEffectorWeight() => effectorWeight;
}