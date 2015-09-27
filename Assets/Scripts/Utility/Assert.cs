using System;
using System.Diagnostics;

public static class Assert
{
  [Conditional("UNITY_EDITOR")]
  public static void IsNotNull<T>(T value)
    => IsNotNull(value, nameof(value));

  [Conditional("UNITY_EDITOR")]
  public static void IsNotNull<T>(T value, string parameterName)
  {
    if (value == null)
      throw new ArgumentNullException($"{parameterName} should never be null!");
  }

  [Conditional("UNITY_EDITOR")]
  public static void AreEqual<T>(T expected, T actual)
    => AreEqual(expected, actual, $"Parameter should always equal {expected}, but was {actual}!");

  [Conditional("UNITY_EDITOR")]
  public static void AreEqual<T>(T expected, T actual, string message)
  {
    if (!Equals(expected, actual))
      throw new ArgumentOutOfRangeException(nameof(expected), actual, message);
  }

  [Conditional("UNITY_EDITOR")]
  public static void IsLessThan(int parameter, int value)
    => IsLessThan((float)parameter, value);

  [Conditional("UNITY_EDITOR")]
  public static void IsLessThan(float parameter, float value)
    => IsLessThan(parameter, value, $"{parameter} should always be less than {value}, but was {parameter}!");

  [Conditional("UNITY_EDITOR")]
  public static void IsLessThan(float parameter, float value, string message)
  {
    if (parameter >= value)
      throw new ArgumentOutOfRangeException(nameof(parameter), parameter, message);
  }

  [Conditional("UNITY_EDITOR")]
  public static void IsGreaterThan(int parameter, int value)
    => IsGreaterThan((float)parameter, value);

  [Conditional("UNITY_EDITOR")]
  public static void IsGreaterThan(float parameter, float value)
    => IsGreaterThan(parameter, value, $"{parameter} shoudl always be greater than {value} but was {parameter}!");

  [Conditional("UNITY_EDITOR")]
  public static void IsGreaterThan(float parameter, float value, string message)
  {
    if (parameter <= value)
      throw new ArgumentOutOfRangeException(nameof(parameter), parameter, message);
  }

  [Conditional("UNITY_EDITOR")]
  public static void IsTrue(bool condition)
    => IsTrue(condition, "Condition should always be true!");

  [Conditional("UNITY_EDITOR")]
  public static void IsTrue(bool condition, string message)
  {
    if (!condition)
      throw new ArgumentException(message);
  }

  [Conditional("UNITY_EDITOR")]
  public static void IsFalse(bool condition)
    => IsFalse(condition, "Condition should always be false!");

  [Conditional("UNITY_EDITOR")]
  public static void IsFalse(bool condition, string message)
  {
    if (condition)
      throw new ArgumentException(message);
  }
}
