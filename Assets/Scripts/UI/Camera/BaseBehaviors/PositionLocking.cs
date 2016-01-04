using System.Diagnostics;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Camera/Base Behaviors/Position Locking")]
  public class PositionLocking : MonoBehaviour, ICameraBaseBehavior
  {
    [SerializeField, BitMask] private CameraAxis axis = CameraAxis.Horizontal;

    [Header("Projected Focus")]
    [Tooltip("projected focus will have the camera push ahead in the direction of the current velocity which is averaged over 5 frames")]
    [SerializeField] private bool enableProjectedFocus = false;
    [Tooltip("when projected focus is enabled the multiplier will increase the forward projection")]
    [SerializeField] private float projectedFocusMultiplier = 3f;

    public bool IsEnabled => enabled;

    private bool AffectHorizontal => (this.axis & CameraAxis.Horizontal) == CameraAxis.Horizontal;
    private bool AffectVertical => (this.axis & CameraAxis.Vertical) == CameraAxis.Vertical;
    private bool AffectBothAxis => AffectHorizontal && AffectVertical;

    [Conditional("UNITY_EDITOR")]
    private void Update() { }

    private Vector3 GetCenterBasedOnContraints(Vector3 basePosition, Vector3 targetPosition)
    {
      var centerPos = basePosition;

      centerPos.z = 0f;

      // if we arent contrained to an axis make it match the targetPosition so we dont have any offset in that direction
      if (!AffectHorizontal)
        centerPos.x = targetPosition.x;

      if (!AffectVertical)
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

      if (AffectBothAxis)
        desiredOffset += targetAverageVelocity * Time.deltaTime * this.projectedFocusMultiplier;
      else if (AffectHorizontal)
        desiredOffset.x += targetAverageVelocity.x * Time.deltaTime * this.projectedFocusMultiplier;
      else if (AffectVertical)
        desiredOffset.y += targetAverageVelocity.y * Time.deltaTime * this.projectedFocusMultiplier;

      return desiredOffset;
    }

    #if UNITY_EDITOR
    public void onDrawGizmos(Vector3 basePosition)
    {
      Gizmos.color = new Color(0f, 0.4f, 0.8f);

      var lineWidth = AffectBothAxis
        ? Camera.main.orthographicSize / 5f
        : Camera.main.orthographicSize / 2f;

      if (AffectHorizontal)
        Gizmos.DrawLine(
          basePosition + new Vector3(0f, -lineWidth, 1f),
          basePosition + new Vector3(0f, lineWidth, 1f));

      if (AffectVertical)
        Gizmos.DrawLine(
          basePosition + new Vector3(-lineWidth, 0f, 1f),
          basePosition + new Vector3(lineWidth, 0f, 1f));
    }
    #endif
  }
}
