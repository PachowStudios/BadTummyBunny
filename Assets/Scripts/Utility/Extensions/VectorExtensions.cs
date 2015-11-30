using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public static class VectorExtensions
{
  public static Vector3 ToVector3(this Vector2 parent)
    => parent.ToVector3(0f);

  public static Vector3 ToVector3(this Vector2 parent, float z)
    => new Vector3(parent.x, parent.y, z);

  [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
  public static bool IsZero(this Vector2 parent)
    => parent.x == 0f && parent.y == 0f;

  public static Vector2 Dot(this Vector2 parent, Vector2 other)
    => parent.Dot(other.x, other.y);

  public static Vector2 Dot(this Vector2 parent, float x, float y)
    => new Vector2(parent.x * x, parent.y * y);

  public static float RandomRange(this Vector2 parent)
    => Random.Range(parent.x, parent.y);

  public static Vector3 SetX(this Vector3 vector, float x)
  {
    vector.Set(x, vector.y, vector.z);

    return vector;
  }

  public static Vector3 SetY(this Vector3 vector, float y)
  {
    vector.Set(vector.x, y, vector.z);

    return vector;
  }

  public static Vector3 SetZ(this Vector3 vector, float z)
  {
    vector.Set(vector.x, vector.y, z);

    return vector;
  }

  public static Vector3 AddX(this Vector3 vector, float x)
    => vector.SetX(vector.x + x);

  public static Vector3 AddY(this Vector3 vector, float y)
    => vector.SetY(vector.y + y);

  public static Vector3 AddZ(this Vector3 vector, float z)
    => vector.SetZ(vector.z + z);

  public static Quaternion LookAt2D(this Vector3 parent, Vector3 target)
  {
    var targetPosition = target - parent;
    var angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;

    return Quaternion.Euler(new Vector3(0f, 0f, Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles.z));
  }

  public static Vector3 DirectionToRotation2D(this Vector3 parent)
    => Quaternion.AngleAxis(
      Mathf.Atan2(parent.y, parent.x) * Mathf.Rad2Deg,
      Vector3.forward)
      .eulerAngles;

  public static float DistanceTo(this Vector3 parent, Vector3 target)
    => Mathf.Sqrt(Mathf.Pow(parent.x - target.x, 2) + Mathf.Pow(parent.y - target.y, 2));
}