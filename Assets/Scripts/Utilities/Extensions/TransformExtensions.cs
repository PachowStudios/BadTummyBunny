namespace UnityEngine
{
  public static class TransformExtensions
  {
    public static float DistanceTo(this Transform transform, Transform target)
      => transform.DistanceTo(target.position);

    public static float DistanceTo(this Transform transform, Vector3 target)
      => transform.position.DistanceTo(target);

    public static void Flip(this Transform transform)
      => transform.localScale = transform.localScale.Scale(x: -1f);

    public static void FlipViaRotation(this Transform transform)
    {
      transform.localRotation = transform.localRotation.eulerAngles
        .Add(z: 180f)
        .Transform(z: z => z.RoundToInt())
        .ToQuaternion();
      transform.localScale = transform.localScale.Scale(y: -1f);
    }
  }
}