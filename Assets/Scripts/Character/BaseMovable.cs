using UnityEngine;

public abstract class BaseMovable : MonoBehaviour, IMovable
{
  [Header("Base Movement")]
  [SerializeField] protected float gravity = -35f;
  [SerializeField] protected float moveSpeed = 5f;

  [Header("Base Components")]
  [SerializeField] protected CharacterController2D controller;

  protected Vector3 velocity = default(Vector3);

  private new Collider2D collider;

  public virtual float? MoveSpeedOverride { get; set; }

  public virtual float Gravity => this.gravity;
  public virtual float MoveSpeed => this.moveSpeed;
  public virtual Vector3 Position => transform.position;
  public virtual Vector3 CenterPoint => Collider.bounds.center;
  public virtual Vector3 LastGroundedPosition { get; protected set; }
  public virtual Vector3 Velocity => this.velocity;
  public virtual Vector2 MovementDirection => this.velocity.normalized;
  public virtual Vector2 FacingDirection => new Vector2(transform.localScale.x, 0f);
  public virtual bool IsFacingRight => FacingDirection.x > 0f;
  public virtual bool IsFalling => Velocity.y < 0f;
  public virtual bool IsGrounded => this.controller.IsGrounded;
  public virtual bool WasGrounded => this.controller.WasGroundedLastFrame;
  public virtual LayerMask CollisionLayers => this.controller.PlatformMask;

  public virtual Collider2D Collider => this.GetComponentIfNull(ref this.collider);

  public virtual void Move(Vector3 moveVelocity)
  {
    this.controller.Move(moveVelocity * Time.deltaTime);
    this.velocity = this.controller.Velocity;
  }

  public virtual void Flip() => transform.Flip();

  public virtual bool Jump(float height)
  {
    if (height <= 0f || !IsGrounded)
      return false;

    this.velocity.y = Mathf.Sqrt(2f * height * -this.gravity);

    return true;
  }

  public virtual void ApplyKnockback(Vector2 knockback, Vector2 direction)
  {
    if (knockback.IsZero())
      return;

    knockback.x += Mathf.Sqrt(Mathf.Abs(Mathf.Pow(knockback.x, 2) * -Gravity));

    if (IsGrounded)
      this.velocity.y = Mathf.Sqrt(Mathf.Abs(knockback.y * -Gravity));

    knockback.Scale(direction);

    if (knockback.IsZero())
      return;

    this.velocity += knockback.ToVector3();
    this.controller.Move(this.velocity * Time.deltaTime);
    this.velocity = this.controller.Velocity;
  }

  public virtual void Disable() { }
}
