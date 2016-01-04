using System;

namespace UnityEngine
{
  public static class VectorExtensions
  {
    public static Vector3 ToVector3(this Vector2 vector)
      => vector.ToVector3(0f);

    public static Vector3 ToVector3(this Vector2 vector, float z)
      => new Vector3(vector.x, vector.y, z);

    public static bool IsZero(this Vector2 vector)
      => Math.Abs(vector.x) < 0.0001f
      && Math.Abs(vector.y) < 0.0001f;

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

    public static Vector2 Dot(this Vector2 vector, Vector2 other)
      => vector.Dot(other.x, other.y);

    public static Vector2 Dot(this Vector2 vector, float x, float y)
      => new Vector2(vector.x * x, vector.y * y);

    public static Vector2 Vary(this Vector2 vector, float variance)
      => new Vector2(
        vector.x.Vary(variance),
        vector.y.Vary(variance));

    public static Vector3 Vary(this Vector3 vector, float variance, bool varyZ = false)
      => new Vector3(
        vector.x.Vary(variance),
        vector.y.Vary(variance),
        varyZ ? vector.z.Vary(variance) : vector.z);

    public static float RandomRange(this Vector2 parent)
      => Random.Range(parent.x, parent.y);

    public static Quaternion LookAt2D(this Vector3 parent, Vector3 target)
    {
      var targetPosition = target - parent;

      return Quaternion.Euler(
        Vector3.zero.SetZ(
          Quaternion.AngleAxis(
            Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg,
            Vector3.forward).eulerAngles.z));
    }

    public static Vector3 DirectionToRotation2D(this Vector3 vector)
      => Quaternion.AngleAxis(
        Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg,
        Vector3.forward)
        .eulerAngles;

    public static float DistanceTo(this Vector3 vector, Vector3 target)
      => Mathf.Sqrt((vector.x - target.x).Sqr() + (vector.y - target.y).Sqr());
  }
}