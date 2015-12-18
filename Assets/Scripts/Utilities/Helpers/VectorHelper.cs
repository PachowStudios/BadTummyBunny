using UnityEngine;

namespace PachowStudios
{
  public static class VectorHelper
  {
    public static Vector3 Vector3Range(Vector3 min, Vector3 max)
      => new Vector3(
        Random.Range(min.x, max.x),
        Random.Range(min.y, max.y),
        Random.Range(min.z, max.z));

    public static Vector3 SuperSmoothLerp(Vector3 followOld, Vector3 targetOld, Vector3 targetNew, float elapsedTime, float lerpAmount)
      => (targetNew - ((targetNew - targetOld) / (lerpAmount * elapsedTime)))
        + (((followOld - targetOld) + ((targetNew - targetOld) / (lerpAmount * elapsedTime)))
        * Mathf.Exp(-lerpAmount * elapsedTime));
  }
}