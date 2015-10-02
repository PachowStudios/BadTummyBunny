using UnityEngine;

public static class GizmosEx
{
  public static void DrawArrowTo(Vector3 from, Vector3 to)
    => DrawArrow(from, to - from);

  public static void DrawArrow(Vector3 position, Vector3 direction)
  {
    const float ArrowHeadLength = 0.75f;
    const float ArrowHeadAngle = 20f;
    var headBase = Quaternion.LookRotation(direction) * Vector3.forward * ArrowHeadLength;

    Gizmos.DrawRay(position, direction);
    Gizmos.DrawRay(position + direction, Quaternion.Euler(0, 0, 180 + ArrowHeadAngle) * headBase);
    Gizmos.DrawRay(position + direction, Quaternion.Euler(0, 0, 180 - ArrowHeadAngle) * headBase);
  }
}