using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class EnemyMovementSettings : BaseMovebaleSettings
  {
    public bool DeactivatedOutsideCamera = true;
  }

  public abstract class EnemyMovement : BaseMovable, IActivatable, ITickable, ILateTickable
  {
    private bool isActivated;

    [InjectLocal] private EnemyMovementSettings Config { get; set; }

    [InjectLocal] protected EnemyView EnemyView { get; private set; }

    protected override IView View => EnemyView;

    public bool IsActivated
    {
      get { return this.isActivated || !Config.DeactivatedOutsideCamera; }
      set { this.isActivated = value; }
    }

    public int HorizontalMovement { get; set; }

    protected bool IsMovingRight => HorizontalMovement > 0;
    protected bool IsMovingLeft => HorizontalMovement < 0;

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

      GetMovement();
      ApplyMovement();
    }

    protected virtual void GetMovement()
    {
      if ((IsMovingRight && !IsFacingRight) ||
          (IsMovingLeft && IsFacingRight))
        Flip();
    }

    protected virtual void ApplyMovement()
    {
      var smoothedMovement = IsGrounded ? Config.GroundDamping : Config.AirDamping;

      Velocity = Velocity.SetX(
        Mathf.Lerp(
          Velocity.x,
          HorizontalMovement * (MoveSpeedOverride ?? MoveSpeed),
          smoothedMovement * Time.deltaTime));

      Velocity = Velocity.AddY(Gravity * Time.deltaTime);
      View.CharacterController.Move(Velocity * Time.deltaTime);
      Velocity = View.CharacterController.Velocity;

      if (IsGrounded)
      {
        Velocity = Velocity.SetY(0f);
        LastGroundedPosition = Position;
      }
    }

    public override bool Jump(float height)
    {
      if (!base.Jump(height))
        return false;

      View.Animator.SetTrigger("Jump");

      return true;
    }
  }
}
