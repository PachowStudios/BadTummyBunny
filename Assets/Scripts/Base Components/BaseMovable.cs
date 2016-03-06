using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseMovable<TConfig, TView> : IMovable
    where TConfig : BaseMovableSettings
    where TView : IView
  {
    public virtual Vector3 Velocity { get; protected set; }
    public virtual Vector3 LastGroundedPosition { get; protected set; }

    public virtual bool IsActivated { get; set; } = true;

    public virtual Collider2D Collider => View.Collider;
    public virtual Vector3 Position => View.Transform.position;
    public virtual Vector3 CenterPoint => Collider.bounds.center;
    public virtual Vector2 MovementDirection => Velocity.normalized;
    public virtual Vector2 FacingDirection => new Vector2(View.Transform.localScale.x, 0f);
    public virtual bool IsFacingRight => FacingDirection.x > 0f;
    public virtual bool IsFalling => Velocity.y < 0f;
    public virtual bool IsGrounded => CharacterController.IsGrounded;
    public virtual bool WasGrounded => CharacterController.WasGroundedLastFrame;
    public virtual LayerMask CollisionLayers => CharacterController.PlatformMask;

    public float? MoveSpeedOverride { get; set; }

    public float Gravity => Config.Gravity;
    public float MoveSpeed => MoveSpeedOverride ?? Config.MoveSpeed;

    protected abstract TConfig Config { get; set; }
    protected abstract TView View { get; set; }

    protected CharacterController2D CharacterController => View.CharacterController;
    protected float MovementDamping => IsGrounded ? Config.GroundDamping : Config.AirDamping;

    public virtual Vector3 Move(Vector3 moveVelocity)
      => Velocity = CharacterController.Move(moveVelocity * Time.deltaTime);

    public virtual void SetPosition(Vector3 position)
      => View.Transform.position = position;

    public virtual void Flip()
      => View.Transform.Flip();

    public virtual bool Jump(float height)
    {
      if (height <= 0f || !IsGrounded)
        return false;

      Velocity = Velocity.SetY(Mathf.Sqrt(2f * height * -Gravity));

      return true;
    }

    public virtual void ApplyKnockback(Vector2 knockback, Vector2 direction)
    {
      if (knockback.IsZero())
        return;

      knockback.x += Mathf.Sqrt(Mathf.Abs(knockback.x.Square() * -Gravity));

      if (IsGrounded)
        Velocity = Velocity.SetY(Mathf.Sqrt(Mathf.Abs(knockback.y * -Gravity)));

      knockback.Scale(direction);

      if (!knockback.IsZero())
        Velocity = Move(Velocity + knockback.ToVector3());
    }

    public virtual void Disable() { }
  }
}