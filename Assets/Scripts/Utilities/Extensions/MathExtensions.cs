namespace UnityEngine
{
  public static class MathExtensions
  {
    public static int Sign(this float number)
    => (int)Mathf.Sign(number);

    public static float Abs(this float number)
      => Mathf.Abs(number);

    public static int RoundToInt(this float number)
      => Mathf.RoundToInt(number);

    public static float RoundToFraction(this float number, int denominator)
      => Mathf.RoundToInt(number * denominator) / (float)denominator;

    public static float Clamp01(this float number)
      => Mathf.Clamp01(number);

    public static float GetDecimal(this float number)
    {
      var resultString = "0";
      float result;

      if (number.ToString().Split('.').Length == 2)
        resultString = "0." + number.ToString().Split('.')[1];

      float.TryParse(resultString, out result);

      return result;
    }
  }
}