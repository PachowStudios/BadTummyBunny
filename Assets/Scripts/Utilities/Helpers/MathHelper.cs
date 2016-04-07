using JetBrains.Annotations;
using UnityEngine;

namespace PachowStudios
{
  public static class MathHelper
  {
    [Pure]
    public static int RandomSign()
      => Random.value < 0.5 ? -1 : 1;

    /// <summary>
    /// Scales a number from one range to another.
    /// </summary>
    /// <example>
    /// Converting 5 from the range 0 - 10 to 0 - 50 results in 25.
    /// </example>
    [Pure]
    public static float ConvertRange(float num, float oldMin, float oldMax, float newMin, float newMax)
      => (((((Mathf.Clamp(num, oldMin, oldMax) - oldMin) * newMax) - newMin) / oldMax) - oldMin) + newMin;
  }
}