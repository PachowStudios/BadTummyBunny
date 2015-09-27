using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CueFocusBase : MonoBehaviour, ICameraEffector
{
  [SerializeField] private Collider2D effectorTrigger = null;

  [SerializeField, BitMask] protected CameraAxis axis;
  [SerializeField] protected float effectorWeight = 0.5f;
  [Tooltip("when true, an additional inner ring can be used to have it's own specific weight indpendent of the outer ring")]
  [SerializeField] protected bool onlyTriggerWhenGrounded = false;

  protected Transform trackedTarget;

  protected Collider2D EffectorTrigger => this.effectorTrigger;
  protected virtual Vector3 FocusPosition => transform.position;
  protected bool AffectHorizontal => (this.axis & CameraAxis.Horizontal) == CameraAxis.Horizontal;
  protected bool AffectVertical => (this.axis & CameraAxis.Vertical) == CameraAxis.Vertical;

  [Conditional("UNITY_EDITOR")]
  protected void Update() { }

  protected virtual void OnTriggerEnter2D(Collider2D other)
  {
    if (this.trackedTarget == null && other.tag == Tags.Player &&
        (!this.onlyTriggerWhenGrounded || Player.Instance.Movement.IsGrounded))
      Activate(other.transform);
  }

  protected virtual void OnTriggerStay2D(Collider2D other)
    => OnTriggerEnter2D(other);

  protected virtual void OnTriggerExit2D(Collider2D other)
  {
    if (this.trackedTarget != null && other.tag == Tags.Player)
      Deactivate();
  }

  protected virtual void OnDisable()
    => Deactivate();

  public virtual Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
  {
    var targetPosition = basePosition;

    if (AffectHorizontal)
      targetPosition.x = FocusPosition.x;
    if (AffectVertical)
      targetPosition.y = FocusPosition.y;

    return targetPosition;
  }

  public abstract float GetEffectorWeight();

  protected virtual void Activate(Transform newTransform)
  {
    this.trackedTarget = newTransform;
    CameraController.Instance?.AddCameraEffector(this);
  }

  protected virtual void Deactivate()
  {
    this.trackedTarget = null;
    CameraController.Instance?.RemoveCameraEffector(this);
  }
}
