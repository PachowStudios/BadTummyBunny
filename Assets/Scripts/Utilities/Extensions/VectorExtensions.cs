namespace UnityEngine
{
  public static class VectorExtensions
  {
    public static Vector3 ToVector3(this Vector2 vector)
      => vector.ToVector3(0f);

    public static Vector3 ToVector3(this Vector2 vector, float z)
      => new Vector3(vector.x, vector.y, z);

    public static Quaternion ToQuaternion(this Vector3 vector)
      => Quaternion.Euler(vector);

    public static bool IsZero(this Vector2 vector)
      => vector.x.Abs() < 0.0001f
      && vector.y.Abs() < 0.0001f;

    public static Vector3 SetX(this Vector3 vector, float x)
    {
      vector.x = x;

      return vector;
    }

    public static Vector3 SetY(this Vector3 vector, float y)
    {
      vector.y = y;

      return vector;
    }

    public static Vector3 SetZ(this Vector3 vector, float z)
    {
      vector.z = z;

      return vector;
    }

    public static Vector3 AddX(this Vector3 vector, float x)
      => vector.SetX(vector.x + x);

    public static Vector3 AddY(this Vector3 vector, float y)
      => vector.SetY(vector.y + y);

    public static Vector3 AddZ(this Vector3 vector, float z)
      => vector.SetZ(vector.z + z);

    public static Vector2 Dot(this Vector2 a, Vector2 b)
      => a.Dot(b.x, b.y);

    public static Vector2 Dot(this Vector2 vector, float x, float y)
      => new Vector2(vector.x * x, vector.y * y);

    public static float DistanceTo(this Vector3 vector, Transform transform)
      => vector.DistanceTo(transform.position);

    public static float DistanceTo(this Vector2 a, Vector2 b)
      => Vector2.Distance(a, b);

    public static float DistanceTo(this Vector3 a, Vector3 b)
      => Vector3.Distance(a, b);

    public static Vector2 LerpTo(this Vector2 a, Vector2 b, float t)
      => Vector2.Lerp(a, b, t);

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

    public static Quaternion LookAt2D(this Vector3 vector, Vector3 target)
      => Vector3.zero
        .SetZ((target - vector).DirectionToRotation2D().z)
        .ToQuaternion();

    public static float AngleDegrees(this Vector3 vector)
      => Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

    public static Vector3 DirectionToRotation2D(this Vector3 vector)
      => Quaternion.AngleAxis(vector.AngleDegrees(), Vector3.forward).eulerAngles;
  }
}