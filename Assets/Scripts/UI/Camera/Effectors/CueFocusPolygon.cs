using UnityEngine;

[AddComponentMenu("UI/Camera/Effectors/Cue Focus Polygon")]
[RequireComponent(typeof(PolygonCollider2D))]
public class CueFocusPolygon : CueFocusBase
{
  protected new PolygonCollider2D EffectorTrigger => (PolygonCollider2D)base.EffectorTrigger;

  public override float GetEffectorWeight() => this.effectorWeight;
}
