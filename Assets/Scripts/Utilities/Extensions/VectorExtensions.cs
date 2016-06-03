using System;
using JetBrains.Annotations;

namespace UnityEngine
{
  public static class VectorExtensions
  {
    [Pure]
    public static Vector3 ToVector3(this Vector2 vector)
      => vector.ToVector3(0f);

    [Pure]
    public static Vector3 ToVector3(this Vector2 vector, float z)
      => new Vector3(vector.x, vector.y, z);

    [Pure]
    public static Quaternion ToQuaternion(this Vector3 vector)
      => Quaternion.Euler(vector);

    [Pure]
    public static bool IsZero(this Vector2 vector)
      => vector.x.IsZero() && vector.y.IsZero();

    [Pure]
    public static Vector2 Set(this Vector2 vector, float? x = null, float? y = null)
      => new Vector2(x ?? vector.x, y ?? vector.y);

    [Pure]
    public static Vector3 Set(this Vector3 vector, float? x = null, float? y = null, float? z = null)
      => new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);

    [Pure]
    public static Vector2 Add(this Vector2 vector, float x = 0f, float y = 0f)
      => new Vector2(vector.x + x, vector.y + y);

    [Pure]
    public static Vector3 Add(this Vector3 vector, float x = 0f, float y = 0f, float z = 0f)
      => new Vector3(vector.x + x, vector.y + y, vector.z + z);

    [Pure]
    public static Vector2 Scale(this Vector2 vector, float x = 1f, float y = 1f)
      => Vector2.Scale(vector, new Vector2(x, y));

    [Pure]
    public static Vector3 Scale(this Vector3 vector, float x = 1f, float y = 1f, float z = 1f)
      => Vector3.Scale(vector, new Vector3(x, y, z));

    [Pure]
    public static Vector2 Transform(
      this Vector2 vector,
      [InstantHandle] Func<float, float> x = null,
      [InstantHandle] Func<float, float> y = null)
      => new Vector2(
        x?.Invoke(vector.x) ?? vector.x,
        y?.Invoke(vector.y) ?? vector.y);

    [Pure]
    public static Vector3 Transform(
      this Vector3 vector,
      [InstantHandle] Func<float, float> x = null,
      [InstantHandle] Func<float, float> y = null,
      [InstantHandle] Func<float, float> z = null)
      => new Vector3(
        x?.Invoke(vector.x) ?? vector.x,
        y?.Invoke(vector.y) ?? vector.y,
        z?.Invoke(vector.z) ?? vector.z);

    [Pure]
    public static float DistanceTo(this Vector3 vector, Transform transform)
      => vector.DistanceTo(transform.position);

    [Pure]
    public static float DistanceTo(this Vector2 a, Vector2 b)
      => Vector2.Distance(a, b);

    [Pure]
    public static float DistanceTo(this Vector3 a, Vector3 b)
      => Vector3.Distance(a, b);

    [Pure]
    public static Vector2 RelationTo(this Vector2 a, Vector2 b)
      => new Vector2(
        a.x >= b.x ? 1f : -1f,
        a.y >= b.y ? 1f : -1f);

    [Pure]
    public static Vector3 RelationTo(this Vector3 a, Vector3 b)
      => new Vector3(
        a.x >= b.x ? 1f : -1f,
        a.y >= b.y ? 1f : -1f,
        a.z >= b.z ? 1f : -1f);

    [Pure]
    public static Vector2 LerpTo(this Vector2 a, Vector2 b, float t)
      => Vector2.Lerp(a, b, t);

    [Pure]
    public static float Angle(this Vector3 vector)
      => Vector3.Angle(Vector3.up, vector);

    [Pure]
    public static float AngleTo(this Vector2 from, Vector2 to)
      => Vector2.Angle(from, to);

    [Pure]
    public static float AngleTo(this Vector3 from, Vector3 to)
      => Vector3.Angle(from, to);

    [Pure]
    public static Vector2 Vary(this Vector2 vector, float variance)
      => new Vector2(
        vector.x.Vary(variance),
        vector.y.Vary(variance));

    [Pure]
    public static Vector3 Vary(this Vector3 vector, float variance, bool varyZ = false)
      => new Vector3(
        vector.x.Vary(variance),
        vector.y.Vary(variance),
        varyZ ? vector.z.Vary(variance) : vector.z);

    [Pure]
    public static float RandomRange(this Vector2 parent)
      => Random.Range(parent.x, parent.y);

    /// <summary>
    /// Converts a movement vector to a rotation that faces the movement direction.
    /// </summary>
    /// <remarks>
    /// The rotation is relative to <see cref="Vector3.forward"/>.
    /// </remarks>
    /// <example>
    /// A movement vector of 1,-1,0 results in a 45deg rotation.
    /// </example>
    [Pure]
    public static Quaternion DirectionToRotation2D(this Vector3 vector)
    {
      var angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

      return Quaternion.AngleAxis(angle, Vector3.forward);
    }
  }
}