using System;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player/Control")]
  public sealed class PlayerControl : BaseMovable, IFartInfoProvider,
    IHandles<PlayerCarrotTriggeredMessage>,
    IHandles<PlayerFlagpoleTriggeredMessage>
  {
    [Header("Movement")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float groundDamping = 10f;
    [SerializeField] private float airDamping = 5f;

    [Header("Farting")]
    [SerializeField] private GameObject startingFart = null;
    [SerializeField] private float maxAvailableFart = 10f;
    [SerializeField] private float fartRechargePerSecond = 1f;
    [SerializeField] [Range(0f, 1f)] private float carrotFartRechargePercent = 0.25f;
    [SerializeField] private float fartDeadZone = 0.2f;
    [SerializeField] private Vector2 fartUsageRange = default(Vector2);

    [Header("Components")]
    [SerializeField] private Transform body = null;
    [SerializeField] private Transform fartPoint = null;

    private float horizontalMovement;
    private bool willJump;

    private IFart currentFart;
    private bool isFartingEnabled = true;
    private float availableFart;
    private float fartingTime;

    private PlayerActions playerActions;
    private Animator animator;

    [Inject(Tags.Player)]
    private IHasHealth Health { get; set; }

    [Inject]
    private IInstantiator Instantiator { get; set; }

    public override Vector2 FacingDirection => new Vector2(this.body.localScale.x, 0f);

    public bool IsFarting { get; private set; }
    public bool IsFartCharging { get; private set; }
    public bool WillFart { get; private set; }
    public Vector2 FartDirection { get; private set; }
    public float FartPower { get; private set; }

    private bool IsInputEnabled { get; set; } = true;

    private bool IsMovingRight => this.horizontalMovement > 0f;
    private bool IsMovingLeft => this.horizontalMovement < 0f;
    private bool CanFart => this.isFartingEnabled && (this.availableFart >= this.fartUsageRange.y);

    private Animator Animator => this.GetComponentIfNull(ref this.animator);

    public override void Flip() => this.body.Flip();

    public override bool Jump(float height)
    {
      if (!base.Jump(height))
        return false;

      Animator.SetTrigger("Jump");

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

    [PostInject]
    private void PostInject()
      => SetFart(this.startingFart);

    private void Awake()
    {
      Assert.IsNotNull(this.startingFart, "No starting fart assigned.");

      this.availableFart = this.maxAvailableFart;
    }

    private void OnEnable()
      => this.playerActions = PlayerActions.CreateWithDefaultBindings();

    private void OnDisable()
      => this.playerActions.Destroy();

    private void Update()
    {
      GetInput();
      ApplyAnimation();

      if (IsGrounded && !WasGrounded)
        PlayLandingSound();
    }

    private void LateUpdate()
    {
      if (Health.IsDead)
        return;

      GetMovement();
      // Disabled due to strange Vectrosity performance
      //UpdateFartTrajectory();
      ApplyMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (IsFarting
          && this.fartingTime > 0.05f
          && CollisionLayers.ContainsLayer(other))
        StopFart(!IsGrounded);
    }

    private void OnTriggerStay2D(Collider2D other)
      => OnTriggerEnter2D(other);

    private void GetInput()
    {
      if (!this.IsInputEnabled)
        return;

      this.horizontalMovement = this.playerActions.Move.Value;
      this.willJump = this.playerActions.Jump.WasPressed && IsGrounded;

      IsFartCharging = this.playerActions.Fart.IsPressed && CanFart;

      if (IsFartCharging)
      {
        var rawFartMagnitude = this.playerActions.Fart.Value.magnitude;

        if (rawFartMagnitude <= this.fartDeadZone)
        {
          IsFartCharging = false;
          FartDirection = Vector2.zero;
          FartPower = 0f;
        }
        else
        {
          FartDirection = this.playerActions.Fart.Value.normalized;
          FartPower = Mathf.InverseLerp(this.fartDeadZone, 1f, rawFartMagnitude);
        }
      }

      if (!IsFartCharging)
        WillFart = this.playerActions.Fart.WasReleased && CanFart;
    }

    private void DisableInput()
    {
      this.IsInputEnabled = false;
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
      Animator.SetBool("Walking", Math.Abs(this.horizontalMovement) > 0.01f && !IsFarting);
      Animator.SetBool("Grounded", IsGrounded);
      Animator.SetBool("Falling", Velocity.y < 0f);
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
        Jump(this.jumpHeight);

      if (!this.isFartingEnabled && IsGrounded)
        this.isFartingEnabled = true;

      this.availableFart = Mathf.Min(this.availableFart + (Time.deltaTime * this.fartRechargePerSecond), this.maxAvailableFart);
    }

    private void ApplyMovement()
    {
      if (IsFarting)
      {
        this.body.CorrectScaleForRotation(Velocity.DirectionToRotation2D());
        this.fartingTime += Time.deltaTime;
      }
      else
      {
        var smoothedMovement = IsGrounded ? this.groundDamping : this.airDamping;

        Velocity = Velocity.SetX(
          Mathf.Lerp(
            Velocity.x,
            this.horizontalMovement * MoveSpeed,
            smoothedMovement * Time.deltaTime));
      }

      Velocity = Velocity.AddY(Gravity * Time.deltaTime);
      Controller.Move(Velocity * Time.deltaTime);
      Velocity = Controller.Velocity;

      if (IsGrounded)
      {
        Velocity = Velocity.SetY(0f);
        LastGroundedPosition = transform.position;
      }
    }

    private void SetFart(GameObject newFart)
    {
      if (newFart == null)
        return;

      var fartInstance = Instantiator.InstantiatePrefab(newFart);
      var fartComponent = fartInstance.GetInterface<IFart>();

      fartInstance.name = fartComponent.FartName;
      fartInstance.transform.position = this.fartPoint.position;
      fartInstance.transform.rotation = this.fartPoint.rotation;
      fartInstance.transform.parent = this.body;

      ((MonoBehaviour)this.currentFart)?.DestroyGameObject();
      this.currentFart = fartComponent;
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

    private float CalculateFartUsage(float fartPower) => Extensions.ConvertRange(fartPower, 0f, 1f, this.fartUsageRange.x, this.fartUsageRange.y);

    private void CollectCarrot(Carrot carrot)
    {
      if (carrot == null)
        return;

      carrot.Collect();
      this.availableFart = Mathf.Min(this.availableFart + (this.maxAvailableFart * this.carrotFartRechargePercent), this.maxAvailableFart);
    }

    private void ActivateLevelFlagpole(Flagpole flagpole)
    {
      if (flagpole.Activated)
        return;

      flagpole.Activate();
      DisableInput();
      Wait.ForSeconds(1.2f, GameMenu.Instance.ShowGameOver);
    }

    private void ResetOrientation()
    {
      var zRotation = this.body.rotation.eulerAngles.z;
      var flipX = zRotation > 90f && zRotation < 270f;
      this.body.localScale = new Vector3(flipX ? -1f : 1f, 1f, 1f);
      this.body.rotation = Quaternion.identity;
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
        SoundManager.PlayCappedSFXFromGroup(rightStep == 1 ? SfxGroups.WalkingGrassRight : SfxGroups.WalkingGrassLeft);
    }

    private void PlayLandingSound()
      => SoundManager.PlayCappedSFXFromGroup(SfxGroups.LandingGrass);

    public void Handle(PlayerCarrotTriggeredMessage message)
      => CollectCarrot(message.Carrot);

    public void Handle(PlayerFlagpoleTriggeredMessage message)
      => ActivateLevelFlagpole(message.Flagpole);
  }
}
