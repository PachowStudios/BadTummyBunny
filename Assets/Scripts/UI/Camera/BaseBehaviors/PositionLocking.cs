using System.Diagnostics;
using UnityEngine;

public class PositionLocking : MonoBehaviour, ICameraBaseBehavior
{
  [SerializeField, BitMask] private CameraAxis axis = CameraAxis.Horizontal;

  [Header("Projected Focus")]
  [Tooltip("projected focus will have the camera push ahead in the direction of the current velocity which is averaged over 5 frames")]
  [SerializeField] private bool enableProjectedFocus = false;
  [Tooltip("when projected focus is enabled the multiplier will increase the forward projection")]
  [SerializeField] private float projectedFocusMultiplier = 3f;

  public bool IsEnabled => enabled;

  [Conditional("UNITY_EDITOR")]
  private void Update() { }

  private Vector3 GetCenterBasedOnContraints(Vector3 basePosition, Vector3 targetPosition)
  {
    var centerPos = basePosition;

    centerPos.z = 0f;

    // if we arent contrained to an axis make it match the targetPosition so we dont have any offset in that direction
    if ((this.axis & CameraAxis.Horizontal) != CameraAxis.Horizontal)
      centerPos.x = targetPosition.x;

    if ((this.axis & CameraAxis.Vertical) != CameraAxis.Vertical)
      centerPos.y = targetPosition.y;

    return centerPos;
  }

  public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
  {
    var centerPos = GetCenterBasedOnContraints(basePosition, targetBounds.center);
    var desiredOffset = targetBounds.center - centerPos;

    // projected focus uses the velocity to project forward
    // TODO: this needs proper smoothing. it only uses the avg velocity right now which can jump around
    if (!this.enableProjectedFocus)
      return desiredOffset;

    var hasHorizontal = (this.axis & CameraAxis.Horizontal) == CameraAxis.Horizontal;
    var hasVertical = (this.axis & CameraAxis.Vertical) == CameraAxis.Vertical;
    var hasBothAxis = hasHorizontal && hasVertical;

    if (hasBothAxis)
      desiredOffset += targetAverageVelocity * Time.deltaTime * this.projectedFocusMultiplier;
    else if (hasHorizontal)
      desiredOffset.x += targetAverageVelocity.x * Time.deltaTime * this.projectedFocusMultiplier;
    else if (hasVertical)
      desiredOffset.y += targetAverageVelocity.y * Time.deltaTime * this.projectedFocusMultiplier;

    return desiredOffset;
  }

  #if UNITY_EDITOR
  public void onDrawGizmos(Vector3 basePosition)
  {
    Gizmos.color = new Color(0f, 0.4f, 0.8f);

    var hasHorizontal = (this.axis & CameraAxis.Horizontal) == CameraAxis.Horizontal;
    var hasVertical = (this.axis & CameraAxis.Vertical) == CameraAxis.Vertical;

    var lineWidth = hasHorizontal && hasVertical ? Camera.main.orthographicSize / 5f 
                                                 : Camera.main.orthographicSize / 2f;

    if (hasVertical)
      Gizmos.DrawLine(basePosition + new Vector3(-lineWidth, 0f, 1f),
                      basePosition + new Vector3(lineWidth, 0f, 1f));

    if (hasHorizontal)
      Gizmos.DrawLine(basePosition + new Vector3(0f, -lineWidth, 1f),
                      basePosition + new Vector3(0f, lineWidth, 1f));
  }
  #endif
}
