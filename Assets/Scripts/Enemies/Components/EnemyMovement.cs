using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class EnemyMovement : BaseMovable, ITickable, ILateTickable
  {
    private bool isActivated;

    public override bool IsActivated
    {
      get { return this.isActivated || !Config.DeactivatedOutsideCamera; }
      set { this.isActivated = value; }
    }

    protected int HorizontalMovement { get; set; }

    protected bool IsWalking => HorizontalMovement != 0;
    protected bool IsFacingMovementDirection
      => (HorizontalMovement >= 0 && View.IsFacingRight)
      || (HorizontalMovement <= 0 && !View.IsFacingRight);

    private BaseEnemyMovementSettings Config { get; }
    private EnemyView View { get; }

    protected EnemyMovement(BaseEnemyMovementSettings config, EnemyView view)
      : base(config, view)
    {
      Config = config;
      View = view;
    }

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
        View.Flip();
    }

    protected virtual void ApplyMovement()
    {
      Move(Velocity
        .Set(x: Velocity.x.LerpTo(
          HorizontalMovement * MoveSpeed,
          MovementDamping * Time.deltaTime))
        .Add(y: Gravity * Time.deltaTime));

      if (IsGrounded)
      {
        Velocity = Velocity.Set(y: 0f);
        LastGroundedPosition = View.Position;
      }
    }
  }
}
