using System;
using JetBrains.Annotations;

namespace UnityEngine
{
  public static class MathExtensions
  {
    public const float FloatingPointTolerance = 0.0001f;

    [Pure]
    public static bool IsZero(this float value)
      => value.IsApproximately(0f);

    [Pure]
    public static bool IsApproximately(this float value, float otherValue)
      => Math.Abs(value - otherValue) < FloatingPointTolerance;

    [Pure]
    public static int Sign(this float value)
      => (int)Mathf.Sign(value);

    [Pure]
    public static float Abs(this float value)
      => Mathf.Abs(value);

    [Pure]
    public static float Square(this float value)
      => Mathf.Pow(value, 2);

    [Pure]
    public static float SquareRoot(this float value)
      => Mathf.Sqrt(value);

    [Pure]
    public static int RoundToInt(this float value)
      => Mathf.RoundToInt(value);

    [Pure]
    public static float RoundToFraction(this float value, int denominator)
      => Mathf.RoundToInt(value * denominator) / (float)denominator;

    [Pure]
    public static int Clamp(this int value, int min, int max)
      => Mathf.Clamp(value, min, max);

    [Pure]
    public static float Clamp(this float value, float min, float max)
      => Mathf.Clamp(value, min, max);

    [Pure]
    public static float Clamp01(this float value)
      => Mathf.Clamp01(value);

    [Pure]
    public static float LerpTo(this float a, float b, float t)
      => Mathf.Lerp(a, b, t);

    [Pure]
    public static float Vary(this float value, float variance)
      => Random.Range(value - variance, value + variance);
  }
}