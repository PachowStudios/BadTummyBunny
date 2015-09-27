using System.Diagnostics;
using UnityEngine;

public class DualForwardFocus : MonoBehaviour, ICameraBaseBehavior
{
  public enum DualForwardFocusType
  {
    ThresholdBased,
    VelocityBased,
    DirectionBased
  }

  [SerializeField, Range(0f, 20f)] private float width = 3f;
  [SerializeField] private DualForwardFocusType dualForwardFocusType = DualForwardFocusType.DirectionBased;

  [Header("Threshold Based")]
  [SerializeField, Range(0.5f, 5f)] private float dualForwardFocusThresholdExtents = 0.5f;

  [Header("Velocity Based")]
  [SerializeField] private float velocityInfluenceMultiplier = 3f;

  private RectTransform.Edge currentEdgeFocus;

  public bool IsEnabled => enabled;

  [Conditional("UNITY_EDITOR")]
  private void Update() { }

  public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
  {
    var desiredOffset = Vector3.zero;

    if (this.dualForwardFocusType == DualForwardFocusType.ThresholdBased)
    {
      var deltaPositionFromBounds = Vector3.zero;
      var didLastEdgeContactChange = false;
      float leftEdge, rightEdge;

      if (this.currentEdgeFocus == RectTransform.Edge.Left)
      {
        rightEdge = basePosition.x - this.width * 0.5f;
        leftEdge = rightEdge - this.dualForwardFocusThresholdExtents * 0.5f;
      }
      else
      {
        leftEdge = basePosition.x + this.width * 0.5f;
        rightEdge = leftEdge + this.dualForwardFocusThresholdExtents * 0.5f;
      }

      if (leftEdge > targetBounds.center.x)
      {
        deltaPositionFromBounds.x = targetBounds.center.x - leftEdge;

        if (this.currentEdgeFocus == RectTransform.Edge.Left)
        {
          didLastEdgeContactChange = true;
          this.currentEdgeFocus = RectTransform.Edge.Right;
        }
      }
      else if (rightEdge < targetBounds.center.x)
      {
        deltaPositionFromBounds.x = targetBounds.center.x - rightEdge;

        if (this.currentEdgeFocus == RectTransform.Edge.Right)
        {
          didLastEdgeContactChange = true;
          this.currentEdgeFocus = RectTransform.Edge.Left;
        }
      }

      var desiredX = this.currentEdgeFocus == RectTransform.Edge.Left ? rightEdge : leftEdge;
      desiredOffset.x = targetBounds.center.x - desiredX;

      // if we didnt switch direction this works much like a normal camera window
      if (!didLastEdgeContactChange)
        desiredOffset.x = deltaPositionFromBounds.x;
    }
    else // velocity or direction based
    {
      var averagedHorizontalVelocity = targetAverageVelocity.x;

      // direction switches are determined by velocity
      if (averagedHorizontalVelocity > 0f)
        this.currentEdgeFocus = RectTransform.Edge.Left;
      else if (averagedHorizontalVelocity < 0f)
        this.currentEdgeFocus = RectTransform.Edge.Right;

      var desiredX = this.currentEdgeFocus == RectTransform.Edge.Left ? basePosition.x - this.width * 0.5f : basePosition.x + this.width * 0.5f;
      desiredX = targetBounds.center.x - desiredX;

      if (this.dualForwardFocusType == DualForwardFocusType.DirectionBased)
        desiredOffset.x = desiredX;
      else
      {
        var velocityMultiplier = Mathf.Max(1f, Mathf.Abs(averagedHorizontalVelocity));
        desiredOffset.x = Mathf.Lerp(0f, desiredX, Time.deltaTime * this.velocityInfluenceMultiplier * velocityMultiplier);
      }
    }

    return desiredOffset;
  }

  #if UNITY_EDITOR
  public void onDrawGizmos(Vector3 basePosition)
  {
    Gizmos.color = new Color(0f, 0.5f, 0.6f);

    var bounds = new Bounds(basePosition, new Vector3(this.width, 10f));
    var lineWidth = Camera.main.orthographicSize;

    bounds.center = new Vector3(bounds.center.x, basePosition.y, bounds.center.z);
    bounds.Expand(new Vector3(0f, lineWidth - bounds.size.y));

    Gizmos.DrawLine(bounds.min, bounds.min + new Vector3(0f, bounds.size.y));
    Gizmos.DrawLine(bounds.max, bounds.max - new Vector3(0f, bounds.size.y));

    if (this.dualForwardFocusType != DualForwardFocusType.ThresholdBased)
      return;

    bounds.Expand(new Vector3(this.dualForwardFocusThresholdExtents, 1f));
    Gizmos.color = Color.blue;
    Gizmos.DrawLine(bounds.min, bounds.min + new Vector3(0f, bounds.size.y));
    Gizmos.DrawLine(bounds.max, bounds.max - new Vector3(0f, bounds.size.y));
  }
  #endif
}
