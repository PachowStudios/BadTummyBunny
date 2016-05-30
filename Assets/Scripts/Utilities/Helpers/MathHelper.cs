using JetBrains.Annotations;
using UnityEngine;

namespace PachowStudios
{
  public static class MathHelper
  {
    /// <summary>
    /// Equivalent to <c>cos(45rad * (π / 180))</c>
    /// </summary>
    /// <remarks>
    /// Also represented as <c>1 / sqrt(2)</c>
    /// </remarks>
    public const float Cos45Deg = 0.707106769f;

    [Pure]
    public static int RandomSign()
      => Random.value < 0.5 ? -1 : 1;

    /// <summary>
    /// Linearly interpolates a value from one range to another.
    /// Clamps the input value to the original range inclusively.
    /// </summary>
    /// <example>
    /// Lerping 12.5 from [10, 15] to [20, 30] results in 25.
    /// </example>
    [Pure]
    public static float LerpRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
      var oldRange = oldMax - oldMin;
      var newRange = newMax - newMin;

      return (((value.Clamp(oldMin, oldMax) - oldMin) * newRange) / oldRange) + newMin;
    }
  }
}