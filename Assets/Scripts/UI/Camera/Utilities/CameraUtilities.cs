using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CameraUtilities
{
  public static void DrawDesiredPositionGizmo(Vector3 position, Color color = default(Color))
  {
    position.z = 0f;

    if (color == default(Color))
      color = Color.green;

    Gizmos.color = color;

    var size = Camera.main.orthographicSize * 0.04f;
    var verticalOffset = new Vector3(0f, size, 0f);
    var horizontalOffset = new Vector3(size, 0f, 0f);

    Gizmos.DrawLine(position - verticalOffset, position + verticalOffset);
    Gizmos.DrawLine(position - horizontalOffset, position + horizontalOffset);
  }

  #if UNITY_EDITOR
  public static void DrawCurrentPositionGizmo(Vector3 position, Color color = default(Color))
  {
    position.z = 0f;

    if (color == default(Color))
      color = Color.yellow;

    var size = Camera.main.orthographicSize * 0.04f;

    Handles.color = color;
    Handles.DrawWireDisc(position, Vector3.back, size);
  }
  #endif

  public static void DrawForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Gizmos.DrawRay(pos, direction);
    DrawArrowEnd(true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
  }

  public static void DrawForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Gizmos.DrawRay(pos, direction);
    DrawArrowEnd(true, pos, direction, color, arrowHeadLength, arrowHeadAngle);
  }

  public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Debug.DrawRay(pos, direction);
    DrawArrowEnd(false, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
  }

  public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Debug.DrawRay(pos, direction, color);
    DrawArrowEnd(false, pos, direction, color, arrowHeadLength, arrowHeadAngle);
  }

  private static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    var right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0)  * Vector3.back;
    var left  = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
    var up    = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0)  * Vector3.back;
    var down  = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

    if (gizmos)
    {
      Gizmos.color = color;
      Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
      Gizmos.DrawRay(pos + direction, left  * arrowHeadLength);
      Gizmos.DrawRay(pos + direction, up    * arrowHeadLength);
      Gizmos.DrawRay(pos + direction, down  * arrowHeadLength);
    }
    else
    {
      Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
      Debug.DrawRay(pos + direction, left  * arrowHeadLength, color);
      Debug.DrawRay(pos + direction, up    * arrowHeadLength, color);
      Debug.DrawRay(pos + direction, down  * arrowHeadLength, color);
    }
  }
}
