namespace UnityEngine
{
  public static class TransformExtensions
  {
    public static void Flip(this Transform parent)
    => parent.localScale = new Vector3(-parent.localScale.x, parent.localScale.y, parent.localScale.z);

    public static Vector3 TransformPointLocal(this Transform parent, Vector3 target)
      => parent.TransformPoint(target) - parent.position;

    public static void CorrectScaleForRotation(this Transform parent, Vector3 target, bool correctY = false)
    {
      var flipY = target.z > 90f && target.z < 270f;

      target.y = correctY && flipY ? 180f : 0f;

      var newScale = parent.localScale;

      newScale.x = 1f;
      newScale.y = flipY ? -1f : 1f;
      parent.localScale = newScale;
      parent.rotation = Quaternion.Euler(target);
    }
  }
}