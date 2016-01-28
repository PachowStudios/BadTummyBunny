using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class BaseMovableSettings : ScriptableObject
  {
    public float Gravity = -35f;
    public float MoveSpeed = 5f;
    public float GroundDamping = 10f;
    public float AirDamping = 5f;
  }

  public abstract class BaseMovable<TConfig, TView> : IMovable
    where TConfig : BaseMovableSettings
    where TView : IView
  {
    protected abstract TConfig Config { get; set; }
    protected abstract TView View { get; set; }

    public virtual Vector3 Velocity { get; protected set; }
    public virtual Vector3 LastGroundedPosition { get; protected set; }
    public virtual float? MoveSpeedOverride { get; set; }

    public virtual bool IsActivated { get; set; } = true;

    public float Gravity => Config.Gravity;
    public float MoveSpeed => Config.MoveSpeed;

    public virtual Collider2D Collider => View.Collider;
    public virtual Vector3 Position => View.Transform.position;
    public virtual Vector3 CenterPoint => Collider.bounds.center;
    public virtual Vector2 MovementDirection => Velocity.normalized;
    public virtual Vector2 FacingDirection => new Vector2(View.Transform.localScale.x, 0f);
    public virtual bool IsFacingRight => FacingDirection.x > 0f;
    public virtual bool IsFalling => Velocity.y < 0f;
    public virtual bool IsGrounded => View.CharacterController.IsGrounded;
    public virtual bool WasGrounded => View.CharacterController.WasGroundedLastFrame;
    public virtual LayerMask CollisionLayers => View.CharacterController.PlatformMask;

    public virtual void SetPosition(Vector3 position)
      => View.Transform.position = position;

    public virtual void Move(Vector3 moveVelocity)
    {
      View.CharacterController.Move(moveVelocity * Time.deltaTime);
      Velocity = View.CharacterController.Velocity;
    }

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

      if (knockback.IsZero())
        return;

      Velocity += knockback.ToVector3();
      View.CharacterController.Move(Velocity * Time.deltaTime);
      Velocity = View.CharacterController.Velocity;
    }

    public virtual void Disable() { }
  }
}