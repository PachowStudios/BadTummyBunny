using JetBrains.Annotations;
using UnityEngine;

namespace PachowStudios
{
  public static class VectorHelper
  {
    [Pure]
    public static Vector3 SuperSmoothLerp(Vector3 followOld, Vector3 targetOld, Vector3 targetNew, float elapsedTime, float lerpAmount)
      => (targetNew - ((targetNew - targetOld) / (lerpAmount * elapsedTime)))
        + (((followOld - targetOld) + ((targetNew - targetOld) / (lerpAmount * elapsedTime)))
        * Mathf.Exp(-lerpAmount * elapsedTime));
  }
}