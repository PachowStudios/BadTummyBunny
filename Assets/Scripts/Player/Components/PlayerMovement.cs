using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class PlayerMovement : BaseMovable<PlayerMovementSettings, PlayerView>, IFartInfoProvider, IInitializable, ITickable, ILateTickable, IDisposable,
    IHandles<PlayerCollidedMessage>,
    IHandles<PlayerCoinCollectedMessage>,
    IHandles<PlayerCarrotCollectedMessage>,
    IHandles<PlayerFlagpoleActivatedMessage>,
    IHandles<LevelCompletedMessage>
  {
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
    [Inject] private IGameMenu GameMenu { get; set; }
    [Inject] private ILevelCompletionHandler LevelCompletionHandler { get; set; }

    private AnimationController AnimationController { get; set; }

    private bool IsInputEnabled { get; set; } = true;
    private float HorizontalMovement { get; set; }
    private bool WillJump { get; set; }

    private IFart CurrentFart { get; set; }
    private bool IsFartingEnabled { get; set; } = true;
    private float TimeFarting { get; set; }

    private bool IsMovingRight => HorizontalMovement > 0f;
    private bool IsMovingLeft => HorizontalMovement < 0f;
    private bool CanFart => IsFartingEnabled;

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
      GetInput();
      AnimationController.Tick();

      if (IsGrounded && !WasGrounded)
        PlayLandingSound();
    }

    public void LateTick()
    {
      if (Health.IsDead)
        return;

      GetMovement();
      UpdateFartTrajectory();
      ApplyMovement();
    }

    public void Dispose()
      => PlayerInput.Destroy();

    public override void Flip() => View.Body.Flip();

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

    private void GetInput()
    {
      if (!IsInputEnabled)
        return;

      HorizontalMovement = PlayerInput.Move.Value;
      WillJump = PlayerInput.Jump.WasPressed && IsGrounded;

      IsFartAiming = PlayerInput.Fart.IsPressed && CanFart;

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
        WillFart = PlayerInput.Fart.WasReleased && CanFart;
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

    private void GetMovement()
    {
      if (WillFart)
        Fart(FartDirection);

      if (IsFarting)
        return;

      if ((IsMovingRight && !IsFacingRight) ||
          (IsMovingLeft && IsFacingRight))
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
      View.CharacterController.Move(Velocity * Time.deltaTime);
      Velocity = View.CharacterController.Velocity;

      if (IsGrounded)
      {
        Velocity = Velocity.SetY(0f);
        LastGroundedPosition = View.Transform.position;
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
      ResetOrientation();

      if (killXVelocity)
        Velocity = Velocity.SetX(0f);

      Velocity = Velocity.SetY(0f);
    }

    private void ResetOrientation()
    {
      var zRotation = View.Body.rotation.eulerAngles.z;
      var flipX = zRotation > 90f && zRotation < 270f;
      View.Body.localScale = new Vector3(flipX ? -1f : 1f, 1f, 1f);
      View.Body.rotation = Quaternion.identity;
    }

    private void ResetInput()
    {
      HorizontalMovement = 0f;
      WillJump = IsFartAiming = false;
      StopFart();
      UpdateFartTrajectory();
    }

    public void Handle(PlayerCollidedMessage message)
    {
      if (IsFarting
          && TimeFarting > 0.05f
          && CollisionLayers.HasLayer(message.Collider))
        StopFart(!IsGrounded);
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
    {
      DisableInput();
      Wait.ForSeconds(1.2f, () => GameMenu.ShowGameOverScreen = true);
    }
  }
}
