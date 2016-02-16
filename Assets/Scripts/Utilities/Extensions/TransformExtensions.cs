namespace UnityEngine
{
  public static class TransformExtensions
  {
    public static float DistanceTo(this Transform transform, Transform target)
      => transform.DistanceTo(target.position);

    public static float DistanceTo(this Transform transform, Vector3 target)
      => transform.position.DistanceTo(target);

    public static void Flip(this Transform parent)
    => parent.localScale = new Vector3(-parent.localScale.x, parent.localScale.y, parent.localScale.z);

    public static Vector3 TransformPointLocal(this Transform parent, Vector3 target)
      => parent.TransformPoint(target) - parent.position;

    public static void CorrectScaleForRotation(this Transform parent, Vector3 target, bool correctY = false)
    {
      var flipY = target.z > 90f && target.z < 270f;

      parent.localScale = new Vector3(
        1f,
        flipY ? -1f : 1f,
        parent.localScale.z);

      parent.rotation = Quaternion.Euler(
        target.SetY(correctY && flipY ? 180f : 0f));
    }
  }
}