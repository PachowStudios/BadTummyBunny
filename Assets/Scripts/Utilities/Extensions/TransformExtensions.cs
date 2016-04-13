using JetBrains.Annotations;
using PachowStudios;

namespace UnityEngine
{
  public static class TransformExtensions
  {
    [Pure]
    public static float DistanceTo([NotNull] this Transform transform, [NotNull] Transform target)
      => transform.DistanceTo(target.position);

    [Pure]
    public static float DistanceTo([NotNull] this Transform transform, Vector3 target)
      => transform.position.DistanceTo(target);

    [Pure]
    public static Vector3 TransformPoint([NotNull] this Transform transform, float x, float y)
      => transform.TransformPoint(x, y, 0f);

    public static void Flip([NotNull] this Transform transform)
      => transform.localScale = transform.localScale.Scale(x: -1f);

    public static void FlipViaRotation([NotNull] this Transform transform)
    {
      transform.localRotation *= Quaternion.Euler(0f, 0f, 180f);
      transform.localScale = transform.localScale.Scale(y: -1f);
    }

    /// <summary>
    /// Sets the position, rotation, and localScale to that of the target transform.
    /// </summary>
    public static void AlignWith([NotNull] this Transform transform, [NotNull] Transform target)
    {
      transform.position = target.position;
      transform.rotation = target.rotation;
      transform.localScale = target.localScale;
    }

    /// <summary>
    /// Determines if the rotation of a transform is between 90 and 270 degrees on the z-axis.
    /// </summary>
    public static bool IsFlippedOnZAxis([NotNull] this Transform transform)
      => transform.localRotation.z.Abs() >= MathHelper.Cos45Deg;
  }
}