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

    public int HorizontalMovement { get; protected set; }

    protected bool IsWalking => HorizontalMovement != 0;

    private BaseEnemyMovementSettings Config { get; }
    private EnemyView View { get; }

    protected EnemyMovement(BaseEnemyMovementSettings config, EnemyView view)
      : base(config, view)
    {
      Config = config;
      View = view;
    }

    public void Tick()
    {
      if (IsActivated)
        InternalTick();
    }

    protected abstract void InternalTick();

    public void LateTick()
    {
      if (IsActivated)
        InternalLateTick();
    }

    protected virtual void InternalLateTick()
      => ApplyMovement();

    public override bool Jump(float height)
    {
      if (!base.Jump(height))
        return false;

      View.Animator.SetTrigger("Jump");

      return true;
    }

    protected virtual void ApplyMovement()
    {
      Move(Velocity
        .Transform(x => x.LerpTo(HorizontalMovement * MoveSpeed, MovementDamping * Time.deltaTime))
        .Add(y: Gravity * Time.deltaTime));

      if (IsGrounded)
      {
        Velocity = Velocity.Set(y: 0f);
        LastGroundedPosition = View.Position;
      }
    }

    protected void Reverse()
      => HorizontalMovement *= -1;
  }
}
