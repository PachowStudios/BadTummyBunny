using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class PlayerMovement : BaseMovable, IFartInfoProvider, IInitializable, ITickable, ILateTickable, IDisposable,
    IHandles<PlayerCoinCollectedMessage>,
    IHandles<PlayerCarrotCollectedMessage>,
    IHandles<PlayerFlagpoleActivatedMessage>,
    IHandles<LevelCompletedMessage>
  {
    private const float MinimumTimeFarting = 0.05f;
    private const float VerticalSlopeAngle = 10f;

    private float fartPower;

    public bool IsFartAiming { get; private set; }
    public Vector2 FartDirection { get; private set; }

    public float FartPower
    {
      get { return this.fartPower; }
      private set { this.fartPower = value.Clamp01(); }
    }

    public bool IsFarting => CurrentFart?.IsFarting ?? false;
    public bool IsSecondaryFarting => CurrentFart?.IsSecondaryFarting ?? false;

    private PlayerMovementSettings Config { get; }
    private PlayerView View { get; }
    private AnimationController AnimationController { get; }

    [InjectLocal] private PlayerInput Input { get; set; }
    [InjectLocal] private IHasHealth Health { get; set; }
    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject(BindingIds.Global)] private IEventAggregator EventAggregator { get; set; }
    [Inject] private IFactory<FartType, IFart> FartFactory { get; set; }
    [Inject] private ILevelCompletionHandler LevelCompletionHandler { get; set; }

    private bool IsInputEnabled { get; set; } = true;
    private bool IsFartingEnabled { get; set; } = true;

    private float HorizontalMovement { get; set; }
    private bool WillJump { get; set; }
    private bool WillFart { get; set; }
    private bool WillSecondaryFart { get; set; }

    private IFart CurrentFart { get; set; }

    private bool CanFart => IsFartingEnabled && (CurrentFart?.CanFart ?? false);
    private float TimeFarting => CurrentFart?.TimeFarting ?? 0f;

    private bool IsWalking => !HorizontalMovement.IsZero() && !IsFarting;
    private bool IsFacingMovementDirection
      => (HorizontalMovement >= 0f && View.IsFacingRight)
      || (HorizontalMovement <= 0f && !View.IsFacingRight);

    public PlayerMovement(PlayerMovementSettings config, PlayerView view)
      : base(config, view)
    {
      Config = config;
      View = view;
      AnimationController = new AnimationController(View.Animator,
        new AnimationCondition("Walking", () => IsWalking),
        new AnimationCondition("Grounded", () => IsGrounded),
        new AnimationCondition("Falling", () => IsFalling));
    }

    [PostInject]
    private void PostInject()
    {
      LocalEventAggregator.Subscribe(this);
      EventAggregator.Subscribe(this);
    }

    public void Initialize()
      => SetFart(Config.StartingFartType);

    public void Tick()
    {
      UpdateInput();
      CurrentFart?.Tick();
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
      => Input.Destroy();

    public override void Move(Vector3 moveVelocity)
    {
      var newVelocity = CharacterController.Move(moveVelocity * Time.deltaTime);

      if (IsFarting
          && TimeFarting > MinimumTimeFarting
          && Velocity.Angle() > VerticalSlopeAngle
          && CharacterController.IsColliding)
        StopFarting(!IsGrounded);
      else
        Velocity = newVelocity;
    }

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
      View.Collider.enabled = false;
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

      HorizontalMovement = Input.Move.Value;
      WillJump = Input.Jump.WasPressed && IsGrounded;

      if (Input.SecondaryFart.IsPressed && CanFart)
      {
        if (Input.SecondaryFart.WasPressed)
          FartPower = 0f;

        FartPower += Time.deltaTime;
      }

      WillSecondaryFart = Input.SecondaryFart.WasReleased && CanFart;
      IsFartAiming = Input.Fart.IsPressed && CanFart;

      if (IsFartAiming)
      {
        var rawFartMagnitude = Input.Fart.Value.magnitude;

        if (rawFartMagnitude <= Config.FartDeadZone)
        {
          IsFartAiming = false;
          FartDirection = Vector2.zero;
          FartPower = 0f;
        }
        else
        {
          FartDirection = Input.Fart.Value.normalized;
          FartPower = Mathf.InverseLerp(Config.FartDeadZone, 1f, rawFartMagnitude);
        }
      }

      if (!IsFartAiming)
        WillFart = Input.Fart.WasReleased && CanFart;
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
        CurrentFart.DrawTrajectory(FartPower, FartDirection, Gravity, View.CenterPoint);
    }

    private void UpdateMovement()
    {
      if (IsFarting)
        return;

      if (WillFart)
        StartFarting();

      if (WillSecondaryFart)
        SecondaryFart();

      if (!IsFacingMovementDirection)
        View.Flip();

      if (WillJump)
        Jump(Config.JumpHeight);

      if (!IsFartingEnabled && IsGrounded)
        IsFartingEnabled = true;
    }

    private void ApplyMovement()
    {
      if (!IsFarting)
        Velocity = Velocity.Set(x: Velocity.x.LerpTo(
          HorizontalMovement * MoveSpeed,
          MovementDamping * Time.deltaTime));

      Move(Velocity.Add(y: Gravity * Time.deltaTime));

      if (IsGrounded)
      {
        Velocity = Velocity.Set(y: 0f);
        LastGroundedPosition = View.Position;
      }
    }

    private void SetFart(FartType type)
    {
      var fart = FartFactory.Create(type);

      fart.Attach(View);
      CurrentFart?.Detach();
      CurrentFart = fart;
    }

    private void StartFarting()
    {
      if (!CanFart || FartDirection == Vector2.zero || FartPower <= 0f)
        return;

      IsFartingEnabled = false;

      Velocity = FartDirection * CurrentFart.CalculateSpeed(FartPower);
      CurrentFart.StartFarting(FartPower);
    }

    private void SecondaryFart()
    {
      if (!CanFart || FartPower <= 0f)
        return;

      CurrentFart.SecondaryFart(FartPower);
    }

    private void StopFarting(bool killXVelocity = true)
    {
      if (!IsFarting)
        return;

      CurrentFart.StopFarting();
      View.ResetOrientation();

      if (killXVelocity)
        Velocity = Velocity.Set(x: 0f);

      Velocity = Velocity.Set(y: 0f);
    }

    private void ResetInput()
    {
      HorizontalMovement = 0f;
      WillJump = IsFartAiming = false;
      StopFarting();
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
