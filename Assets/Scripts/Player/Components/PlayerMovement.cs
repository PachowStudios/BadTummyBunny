using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class PlayerMovement : BaseMovable, IFartInfoProvider, ITickable, ILateTickable, IDisposable,
    IHandles<PlayerCollidedMessage>,
    IHandles<PlayerCarrotTriggeredMessage>,
    IHandles<PlayerFlagpoleTriggeredMessage>
  {
    [InstallerSettings]
    public class Settings : BaseSettings
    {
      [Header("Movement")]
      public float JumpHeight = 5f;

      [Header("Farting")]
      public FartType StartingFartType = FartType.Basic;
      public float MaxAvailableFart = 10f;
      public float FartRechargePerSecond = 1f;
      [Range(0f, 1f)] public float CarrotFartRechargePercent = 0.25f;
      public float FartDeadZone = 0.2f;
      public Vector2 FartUsageRange = default(Vector2);
    }

    private float horizontalMovement;
    private bool willJump;

    private IFart currentFart;
    private bool isFartingEnabled = true;
    private float availableFart;
    private float fartingTime;

    [InjectLocal] private Settings Config { get; set; }
    [InjectLocal] private PlayerInput PlayerInput { get; set; }
    [InjectLocal] private PlayerView PlayerView { get; set; }
    [InjectLocal] private IHasHealth Health { get; set; }
    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject] private IFactory<FartType, IFart> FartFactory { get; set; }
    [Inject] private IGameMenu GameMenu { get; set; }

    public bool IsFarting { get; private set; }
    public bool IsFartCharging { get; private set; }
    public bool WillFart { get; private set; }
    public Vector2 FartDirection { get; private set; }
    public float FartPower { get; private set; }

    private bool IsInputEnabled { get; set; } = true;

    public override Vector2 FacingDirection => new Vector2(PlayerView.Body.localScale.x, 0f);

    protected override IView View => PlayerView;

    private bool IsMovingRight => this.horizontalMovement > 0f;
    private bool IsMovingLeft => this.horizontalMovement < 0f;
    private bool CanFart => this.isFartingEnabled && (this.availableFart >= Config.FartUsageRange.y);

    public PlayerMovement()
    {
      this.availableFart = Config.MaxAvailableFart;
    }

    [PostInject]
    private void Initialize()
    {
      LocalEventAggregator.Subscribe(this);
      SetFart(Config.StartingFartType);
    }

    public void Tick()
    {
      GetInput();
      ApplyAnimation();

      if (IsGrounded && !WasGrounded)
        PlayLandingSound();
    }

    public void LateTick()
    {
      if (Health.IsDead)
        return;

      GetMovement();
      // Disabled due to strange Vectrosity performance
      //UpdateFartTrajectory();
      ApplyMovement();
    }

    public void Dispose()
      => PlayerInput.Destroy();

    public override void Flip() => PlayerView.Body.Flip();

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

    private void GetInput()
    {
      if (!IsInputEnabled)
        return;

      this.horizontalMovement = PlayerInput.Move.Value;
      this.willJump = PlayerInput.Jump.WasPressed && IsGrounded;

      IsFartCharging = PlayerInput.Fart.IsPressed && CanFart;

      if (IsFartCharging)
      {
        var rawFartMagnitude = PlayerInput.Fart.Value.magnitude;

        if (rawFartMagnitude <= Config.FartDeadZone)
        {
          IsFartCharging = false;
          FartDirection = Vector2.zero;
          FartPower = 0f;
        }
        else
        {
          FartDirection = PlayerInput.Fart.Value.normalized;
          FartPower = Mathf.InverseLerp(Config.FartDeadZone, 1f, rawFartMagnitude);
        }
      }

      if (!IsFartCharging)
        WillFart = PlayerInput.Fart.WasReleased && CanFart;
    }

    private void DisableInput()
    {
      IsInputEnabled = false;
      ResetInput();
    }

    private void UpdateFartTrajectory()
    {
      if (IsFartCharging)
        this.currentFart.DrawTrajectory(FartPower, FartDirection, Gravity, CenterPoint);
      else
        this.currentFart.ClearTrajectory();
    }

    private void ApplyAnimation()
    {
      View.Animator.SetBool("Walking", Math.Abs(this.horizontalMovement) > 0.01f && !IsFarting);
      View.Animator.SetBool("Grounded", IsGrounded);
      View.Animator.SetBool("Falling", Velocity.y < 0f);
    }

    private void GetMovement()
    {
      if (WillFart)
        Fart(FartDirection, FartPower);

      if (IsFarting)
        return;

      if (IsMovingRight && !IsFacingRight ||
          (IsMovingLeft && IsFacingRight))
        Flip();

      if (this.willJump)
        Jump(Config.JumpHeight);

      if (!this.isFartingEnabled && IsGrounded)
        this.isFartingEnabled = true;

      this.availableFart = Mathf.Min(this.availableFart + (Time.deltaTime * Config.FartRechargePerSecond), Config.MaxAvailableFart);
    }

    private void ApplyMovement()
    {
      if (IsFarting)
      {
        PlayerView.Body.CorrectScaleForRotation(Velocity.DirectionToRotation2D());
        this.fartingTime += Time.deltaTime;
      }
      else
      {
        var smoothedMovement = IsGrounded ? Config.GroundDamping : Config.AirDamping;

        Velocity = Velocity.SetX(
          Mathf.Lerp(
            Velocity.x,
            this.horizontalMovement * MoveSpeed,
            smoothedMovement * Time.deltaTime));
      }

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

      fart.Attach(PlayerView);
      this.currentFart.Detach();
      this.currentFart = fart;
    }

    private void Fart(Vector2 fartDirection, float fartPower)
    {
      if (IsFarting || FartDirection == Vector2.zero || FartPower <= 0)
        return;

      IsFarting = true;
      this.isFartingEnabled = false;

      this.fartingTime = 0f;
      this.availableFart = Mathf.Max(0f, this.availableFart - CalculateFartUsage(fartPower));
      Velocity = fartDirection * this.currentFart.CalculateSpeed(FartPower);

      this.currentFart.StartFart(FartPower, FartDirection);
    }

    private void StopFart(bool killXVelocity = true)
    {
      if (!IsFarting)
        return;

      IsFarting = false;

      this.currentFart.StopFart();
      ResetOrientation();

      if (killXVelocity)
        Velocity = Velocity.SetX(0f);

      Velocity = Velocity.SetY(0f);
    }

    private float CalculateFartUsage(float fartPower) => MathHelper.ConvertRange(fartPower, 0f, 1f, Config.FartUsageRange.x, Config.FartUsageRange.y);

    private void CollectCarrot(Carrot carrot)
    {
      if (carrot == null)
        return;

      carrot.Collect();
      this.availableFart = Mathf.Min(
        this.availableFart + (Config.MaxAvailableFart * Config.CarrotFartRechargePercent),
        Config.MaxAvailableFart);
    }

    private void ActivateLevelFlagpole(Flagpole flagpole)
    {
      if (flagpole.Activated)
        return;

      flagpole.Activate();
      DisableInput();
      Wait.ForSeconds(1.2f, () => GameMenu.ShowGameOverScreen = true);
    }

    private void ResetOrientation()
    {
      var zRotation = PlayerView.Body.rotation.eulerAngles.z;
      var flipX = zRotation > 90f && zRotation < 270f;
      PlayerView.Body.localScale = new Vector3(flipX ? -1f : 1f, 1f, 1f);
      PlayerView.Body.rotation = Quaternion.identity;
    }

    private void ResetInput()
    {
      this.horizontalMovement = 0f;
      this.willJump = false;
      IsFartCharging = false;
      StopFart();
      UpdateFartTrajectory();
    }

    private void PlayWalkingSound(int rightStep)
    {
      if (!IsFarting && IsGrounded)
        SoundManager.PlayCappedSFXFromGroup(rightStep == 1 ? SfxGroup.WalkingGrassRight : SfxGroup.WalkingGrassLeft);
    }

    public void Handle(PlayerCollidedMessage message)
    {
      if (IsFarting
          && this.fartingTime > 0.05f
          && CollisionLayers.HasLayer(message.Collider))
        StopFart(!IsGrounded);
    }

    public void Handle(PlayerCarrotTriggeredMessage message)
      => CollectCarrot(message.Carrot);

    public void Handle(PlayerFlagpoleTriggeredMessage message)
      => ActivateLevelFlagpole(message.Flagpole);

    private static void PlayLandingSound()
      => SoundManager.PlayCappedSFXFromGroup(SfxGroup.LandingGrass);
  }
}
