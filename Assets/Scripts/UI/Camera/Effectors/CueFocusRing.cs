using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Camera/Effectors/Cue Focus Ring")]
  [RequireComponent(typeof(CircleCollider2D))]
  public sealed class CueFocusRing : CueFocusBase
  {
    [Tooltip("when true, an additional inner ring can be used to have it's own specific weight indpendent of the outer ring")]
    [SerializeField] private bool enableInnerRing = false;
    [SerializeField] private float innerRingRadius = 2f;
    [SerializeField] private float innerEffectorWeight = 5f;
    [SerializeField] private bool enableEffectorFalloff = true;
    [Tooltip("The curve should go from 0 to 1 being the normalized distance from center to radius. It's value will be multiplied by the effectorWeight to get the final weight used.")]
    [SerializeField] private AnimationCurve effectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private CircleCollider2D CircleTrigger => (CircleCollider2D)EffectorTrigger;
    private Vector3 EffectorPosition => transform.TransformPoint(CircleTrigger.offset);

    private float OuterRingRadius => CircleTrigger.radius;

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      Handles.color = Color.green;

      if (CircleTrigger == null)
        return;

      if (this.enableInnerRing)
        Handles.DrawWireDisc(EffectorPosition, Vector3.back, this.innerRingRadius);

      Handles.PositionHandle(EffectorPosition, Quaternion.identity);
    }
    #endif

    public override float GetEffectorWeight()
    {
      var effectorDistance = EffectorPosition.DistanceTo(TrackedTarget);

      if (this.enableInnerRing && effectorDistance <= this.innerRingRadius)
        return this.innerEffectorWeight;

      if (this.enableEffectorFalloff)
        return this.effectorFalloff.Evaluate(1f - (effectorDistance / OuterRingRadius)) * EffectorWeight;

      return EffectorWeight;
    }
  }
}
