using System;

namespace UnityEngine
{
  public static class MathExtensions
  {
    private const float FloatingPointTolerance = 0.0001f;

    public static bool IsZero(this float value)
      => value.Abs() <= FloatingPointTolerance;

    public static bool Is(this float value, float otherValue)
      => Math.Abs(value - otherValue) < FloatingPointTolerance;

    public static int Sign(this float value)
      => (int)Mathf.Sign(value);

    public static float Abs(this float value)
      => Mathf.Abs(value);

    public static float Sqr(this float value)
      => Mathf.Pow(value, 2);

    public static int RoundToInt(this float value)
      => Mathf.RoundToInt(value);

    public static float RoundToFraction(this float value, int denominator)
      => Mathf.RoundToInt(value * denominator) / (float)denominator;

    public static int Clamp(this int value, int min, int max)
      => Mathf.Clamp(value, min, max);

    public static float Clamp(this float value, float min, float max)
      => Mathf.Clamp(value, min, max);

    public static float Clamp01(this float value)
      => Mathf.Clamp01(value);

    public static float Vary(this float value, float variance)
      => Random.Range(value - variance, value + variance);

    public static float GetDecimal(this float value)
    {
      var resultString = "0";
      float result;

      if (value.ToString().Split('.').Length == 2)
        resultString = "0." + value.ToString().Split('.')[1];

      float.TryParse(resultString, out result);

      return result;
    }
  }
}