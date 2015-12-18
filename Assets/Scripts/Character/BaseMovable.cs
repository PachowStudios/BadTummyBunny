using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseMovable : MonoBehaviour, IMovable
  {
    [Header("Base Movement")]
    [SerializeField] private float gravity = -35f;
    [SerializeField] private float moveSpeed = 5f;

    private Collider2D colliderComponent;
    private CharacterController2D controller;

    public virtual Vector3 Velocity { get; protected set; }
    public virtual Vector3 LastGroundedPosition { get; protected set; }
    public virtual float? MoveSpeedOverride { get; set; }

    public virtual float Gravity => this.gravity;
    public virtual float MoveSpeed => this.moveSpeed;
    public virtual Vector3 Position => transform.position;
    public virtual Vector3 CenterPoint => Collider.bounds.center;
    public virtual Vector2 MovementDirection => Velocity.normalized;
    public virtual Vector2 FacingDirection => new Vector2(transform.localScale.x, 0f);
    public virtual bool IsFacingRight => FacingDirection.x > 0f;
    public virtual bool IsFalling => Velocity.y < 0f;
    public virtual bool IsGrounded => Controller.IsGrounded;
    public virtual bool WasGrounded => Controller.WasGroundedLastFrame;
    public virtual LayerMask CollisionLayers => Controller.PlatformMask;

    public virtual Collider2D Collider => this.GetComponentIfNull(ref this.colliderComponent);

    protected CharacterController2D Controller => this.GetComponentIfNull(ref this.controller);

    public virtual void Move(Vector3 moveVelocity)
    {
      Controller.Move(moveVelocity * Time.deltaTime);
      Velocity = Controller.Velocity;
    }

    public virtual void Flip() => transform.Flip();

    public virtual bool Jump(float height)
    {
      if (height <= 0f || !IsGrounded)
        return false;

      Velocity = Velocity.SetY(Mathf.Sqrt(2f * height * -this.gravity));

      return true;
    }

    public virtual void ApplyKnockback(Vector2 knockback, Vector2 direction)
    {
      if (knockback.IsZero())
        return;

      knockback.x += Mathf.Sqrt(Mathf.Abs(Mathf.Pow(knockback.x, 2) * -Gravity));

      if (IsGrounded)
        Velocity = Velocity.SetY(Mathf.Sqrt(Mathf.Abs(knockback.y * -Gravity)));

      knockback.Scale(direction);

      if (knockback.IsZero())
        return;

      Velocity += knockback.ToVector3();
      Controller.Move(Velocity * Time.deltaTime);
      Velocity = Controller.Velocity;
    }

    public virtual void Disable() { }
  }
}