using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class EnemyMovement<TConfig> : BaseMovable<TConfig, EnemyView>, ITickable, ILateTickable
    where TConfig : BaseEnemyMovementSettings
  {
    private bool isActivated;

    public override bool IsActivated
    {
      get { return this.isActivated || !Config.DeactivatedOutsideCamera; }
      set { this.isActivated = value; }
    }

    public int HorizontalMovement { get; set; }

    [InjectLocal] protected override EnemyView View { get; set; }

    private bool IsFacingMovementDirection
      => (HorizontalMovement >= 0 && IsFacingRight)
      || (HorizontalMovement <= 0 && !IsFacingRight);

    protected abstract void InternalTick();

    public void Tick()
    {
      if (IsActivated)
        InternalTick();
    }

    public virtual void LateTick()
    {
      if (!IsActivated)
        return;

      UpdateMovement();
      ApplyMovement();
    }

    public override bool Jump(float height)
    {
      if (!base.Jump(height))
        return false;

      View.Animator.SetTrigger("Jump");

      return true;
    }

    protected virtual void UpdateMovement()
    {
      if (!IsFacingMovementDirection)
        Flip();
    }

    protected virtual void ApplyMovement()
    {
      Move(Velocity
        .SetX(Velocity.x.LerpTo(HorizontalMovement * MoveSpeed, MovementDamping * Time.deltaTime))
        .AddY(Gravity * Time.deltaTime));

      if (IsGrounded)
      {
        Velocity = Velocity.SetY(0f);
        LastGroundedPosition = Position;
      }
    }
  }
}
