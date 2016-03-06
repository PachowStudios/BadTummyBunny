using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class PlayerMovement : BaseMovable<PlayerMovementSettings, PlayerView>, IFartInfoProvider, IInitializable, ITickable, ILateTickable, IDisposable,
    IHandles<PlayerCoinCollectedMessage>,
    IHandles<PlayerCarrotCollectedMessage>,
    IHandles<PlayerFlagpoleActivatedMessage>,
    IHandles<LevelCompletedMessage>
  {
    private const float MinimumTimeFarting = 0.05f;
    private const float VerticalSlopeAngle = 10f;

    public bool IsFarting { get; private set; }
    public bool IsFartAiming { get; private set; }
    public bool WillFart { get; private set; }
    public Vector2 FartDirection { get; private set; }
    public float FartPower { get; private set; }

    public override Vector2 FacingDirection => new Vector2(View.Body.localScale.x, 0f);

    [InjectLocal] protected override PlayerMovementSettings Config { get; set; }
    [InjectLocal] protected override PlayerView View { get; set; }

    [InjectLocal] private PlayerInput PlayerInput { get; set; }
    [InjectLocal] private IHasHealth Health { get; set; }
    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject(BindingIds.Global)] private IEventAggregator EventAggregator { get; set; }
    [Inject] private IFactory<FartType, IFart> FartFactory { get; set; }
    [Inject] private ILevelCompletionHandler LevelCompletionHandler { get; set; }

    private AnimationController AnimationController { get; set; }

    private bool IsInputEnabled { get; set; } = true;
    private bool IsFartingEnabled { get; set; } = true;

    private float HorizontalMovement { get; set; }
    private bool WillJump { get; set; }

    private IFart CurrentFart { get; set; }
    private float TimeFarting { get; set; }

    private bool IsFacingMovementDirection
      => (HorizontalMovement >= 0f && IsFacingRight)
      || (HorizontalMovement <= 0f && !IsFacingRight);

    [PostInject]
    private void PostInject()
    {
      LocalEventAggregator.Subscribe(this);
      EventAggregator.Subscribe(this);
      AnimationController = new AnimationController(View.Animator,
        new AnimationCondition("Walking", () => HorizontalMovement.Abs() > 0.01f && !IsFarting),
        new AnimationCondition("Grounded", () => IsGrounded),
        new AnimationCondition("Falling", () => Velocity.y < 0f));
    }

    public void Initialize()
      => SetFart(Config.StartingFartType);

    public void Tick()
    {
      UpdateInput();
      AnimationController.Tick();

      if (IsGrounded && !WasGrounded)
        PlayLandingSound();
    }

    public void LateTick()
    {
      if (Health.IsDead)
        return;

      UpdateMovement();
      UpdateFartTrajectory();
      ApplyMovement();
    }

    public void Dispose()
      => PlayerInput.Destroy();

    public override Vector3 Move(Vector3 moveVelocity)
    {
      var newVelocity = CharacterController.Move(moveVelocity * Time.deltaTime);

      if (IsFarting
          && TimeFarting > MinimumTimeFarting
          && Velocity.Angle() > VerticalSlopeAngle
          && CharacterController.IsColliding)
        StopFart(!IsGrounded);
      else
        Velocity = newVelocity;

      return Velocity;
    }

    public override void Flip()
      => View.Body.Flip();

    public override bool Jump(float height)
    {
      if (!base.Jump(height))
        return false;

      View.Animator.SetTrigger("Jump");

      return true;
    }

    public override void ApplyKnockback(Vector2 knockback, Vector2 direction)
    {
      if (!IsFarting)
        base.ApplyKnockback(knockback, direction);
    }

    public override void Disable()
    {
      Collider.enabled = false;
      DisableInput();
    }

    public void PlayWalkingSound(bool isRightStep)
    {
      if (!IsFarting && IsGrounded)
        SoundManager.PlayCappedSFXFromGroup(isRightStep ? SfxGroup.WalkingGrassRight : SfxGroup.WalkingGrassLeft);
    }

    private void UpdateInput()
    {
      if (!IsInputEnabled)
        return;

      HorizontalMovement = PlayerInput.Move.Value;
      WillJump = PlayerInput.Jump.WasPressed && IsGrounded;

      IsFartAiming = PlayerInput.Fart.IsPressed && IsFartingEnabled;

      if (IsFartAiming)
      {
        var rawFartMagnitude = PlayerInput.Fart.Value.magnitude;

        if (rawFartMagnitude <= Config.FartDeadZone)
        {
          IsFartAiming = false;
          FartDirection = Vector2.zero;
          FartPower = 0f;
        }
        else
        {
          FartDirection = PlayerInput.Fart.Value.normalized;
          FartPower = Mathf.InverseLerp(Config.FartDeadZone, 1f, rawFartMagnitude);
        }
      }

      if (!IsFartAiming)
        WillFart = PlayerInput.Fart.WasReleased && IsFartingEnabled;
    }

    private void DisableInput()
    {
      IsInputEnabled = false;
      ResetInput();
    }

    private void UpdateFartTrajectory()
    {
      CurrentFart.ShowTrajectory = IsFartAiming;

      if (IsFartAiming)
        CurrentFart.DrawTrajectory(FartPower, FartDirection, Gravity, CenterPoint);
    }

    private void UpdateMovement()
    {
      if (WillFart)
        Fart(FartDirection);

      if (IsFarting)
        return;

      if (!IsFacingMovementDirection)
        Flip();

      if (WillJump)
        Jump(Config.JumpHeight);

      if (!IsFartingEnabled && IsGrounded)
        IsFartingEnabled = true;
    }

    private void ApplyMovement()
    {
      if (IsFarting)
        TimeFarting += Time.deltaTime;
      else
        Velocity = Velocity.SetX(Velocity.x.LerpTo(
          HorizontalMovement * MoveSpeed,
          MovementDamping * Time.deltaTime));

      Velocity = Velocity.AddY(Gravity * Time.deltaTime);
      Move(Velocity);

      if (IsGrounded)
      {
        Velocity = Velocity.SetY(0f);
        LastGroundedPosition = Position;
      }
    }

    private void SetFart(FartType type)
    {
      var fart = FartFactory.Create(type);

      fart.Attach(View);
      CurrentFart?.Detach();
      CurrentFart = fart;
    }

    private void Fart(Vector2 fartDirection)
    {
      if (IsFarting || FartDirection == Vector2.zero || FartPower <= 0)
        return;

      IsFarting = true;
      IsFartingEnabled = false;

      TimeFarting = 0f;
      Velocity = fartDirection * CurrentFart.CalculateSpeed(FartPower);

      CurrentFart.StartFart(FartPower);
    }

    private void StopFart(bool killXVelocity = true)
    {
      if (!IsFarting)
        return;

      IsFarting = false;

      CurrentFart.StopFart();
      View.ResetOrientation();

      if (killXVelocity)
        Velocity = Velocity.SetX(0f);

      Velocity = Velocity.SetY(0f);
    }

    private void ResetInput()
    {
      HorizontalMovement = 0f;
      WillJump = IsFartAiming = false;
      StopFart();
      UpdateFartTrajectory();
    }

    public void Handle(PlayerCoinCollectedMessage message)
      => message.Coin.Collect();

    public void Handle(PlayerCarrotCollectedMessage message)
      => message.Carrot.Collect();

    public void Handle(PlayerFlagpoleActivatedMessage message)
      => message.Flagpole.Activate();

    private static void PlayLandingSound()
      => SoundManager.PlayCappedSFXFromGroup(SfxGroup.LandingGrass);

    public void Handle(LevelCompletedMessage message)
      => DisableInput();
  }
}
