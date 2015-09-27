using System.Diagnostics;
using UnityEngine;

public class MapExtentsFinalizer : MonoBehaviour, ICameraFinalizer
{
  [SerializeField] private bool snapToBottom = false;
  [SerializeField] private bool snapToTop = false;
  [SerializeField] private bool snapToRight = false;
  [SerializeField] private bool snapToLeft = false;

  [SerializeField] private float bottomConstraint = 0f;
  [SerializeField] private float topConstraint = 0f;
  [SerializeField] private float rightConstraint = 0f;
  [SerializeField] private float leftConstraint = 0f;

  public bool IsEnabled => enabled;
  public int GetFinalizerPriority => 0;
  public bool ShouldSkipSmoothingThisFrame => false;

  [Conditional("UNITY_EDITOR")]
  private void Update() { }

  public Vector3 GetFinalCameraPosition(Bounds targetBounds, Vector3 currentCameraPosition, Vector3 desiredCameraPosition)
  {
    var orthoSize = CameraController.Instance.Camera.orthographicSize;
    var orthoHalfWidth = orthoSize * CameraController.Instance.Camera.aspect;

    if (this.snapToLeft && desiredCameraPosition.x - orthoHalfWidth < this.leftConstraint)
      desiredCameraPosition.x = this.leftConstraint + orthoHalfWidth;

    if (this.snapToRight && desiredCameraPosition.x + orthoHalfWidth > this.rightConstraint)
      desiredCameraPosition.x = this.rightConstraint - orthoHalfWidth;

    if (this.snapToTop && desiredCameraPosition.y + orthoSize > this.topConstraint)
      desiredCameraPosition.y = this.topConstraint - orthoSize;

    if (this.snapToBottom && desiredCameraPosition.y - orthoSize < this.bottomConstraint)
      desiredCameraPosition.y = this.bottomConstraint + orthoSize;

    return desiredCameraPosition;
  }

  #if UNITY_EDITOR
  public void onDrawGizmos(Vector3 basePosition)
  {
    const int FakeInfinity = 10000;

    Gizmos.color = Color.red;

    if (this.snapToBottom)
      Gizmos.DrawLine(new Vector2(-FakeInfinity, this.bottomConstraint),
                      new Vector2(FakeInfinity, this.bottomConstraint));
    if (this.snapToTop)
      Gizmos.DrawLine(new Vector2(-FakeInfinity, this.topConstraint),
                      new Vector2(FakeInfinity, this.topConstraint));
    if (this.snapToRight)
      Gizmos.DrawLine(new Vector2(this.rightConstraint, -FakeInfinity),
                      new Vector2(this.rightConstraint, FakeInfinity));
    if (this.snapToLeft)
      Gizmos.DrawLine(new Vector2(this.leftConstraint, -FakeInfinity),
                      new Vector2(this.leftConstraint, FakeInfinity));
  }
  #endif
}
