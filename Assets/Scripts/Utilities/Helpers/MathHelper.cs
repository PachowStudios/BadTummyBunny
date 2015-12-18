using UnityEngine;

namespace PachowStudios
{
  public static class MathHelper
  {
    public static int RandomSign()
      => Random.value < 0.5 ? -1 : 1;

    public static float ConvertRange(float num, float oldMin, float oldMax, float newMin, float newMax)
      => (((((Mathf.Clamp(num, oldMin, oldMax) - oldMin) * newMax) - newMin) / oldMax) - oldMin) + newMin;
  }
}