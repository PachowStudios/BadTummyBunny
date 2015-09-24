using UnityEngine;

[AddComponentMenu("Player/Control")]
public sealed class PlayerControl : BaseMovable, IFartInfoProvider
{
	[Header("Movement")]
	[SerializeField]
	private float jumpHeight = 5f;
	[SerializeField]
	private float groundDamping = 10f;
	[SerializeField]
	private float airDamping = 5f;

	[Header("Farting")]
	[SerializeField]
	private MonoBehaviour startingFart = null;
	[SerializeField]
	private float maxAvailableFart = 10f;
	[SerializeField]
	private float fartRechargePerSecond = 1f;
	[SerializeField]
	[Range(0f, 1f)]
	private float carrotFartRechargePercent = 0.25f;
	[SerializeField]
	private float fartDeadZone = 0.2f;
	[SerializeField]
	private Vector2 fartUsageRange = default(Vector2);

	[Header("Components")]
	[SerializeField]
	private Transform body = null;
	[SerializeField]
	private Transform fartPoint = null;

	private float horizontalMovement = 0f;
	private bool willJump = false;
	private bool willFart = false;
	private bool isInputEnabled = true;

	private IFart currentFart = null;
	private bool isFarting = false;
	private bool isFartingEnabled = true;
	private bool isFartCharging = false;
	private float availableFart = 0f;
	private float fartPower = 0f;
	private Vector2 fartDirection = Vector2.zero;
	private float fartingTime = 0f;

	private PlayerActions playerActions;
	private Animator animator;

	public override Vector2 FacingDirection => new Vector2(body.localScale.x, 0f);

	public bool IsFarting => isFarting;
	public bool IsFartCharging => isFartCharging;
	public bool WillFart => willFart;
	public Vector2 FartDirection => fartDirection;
	public float FartPower => fartPower;
	public float AvailableFartPercent => Mathf.Clamp01(availableFart / maxAvailableFart);

	private bool IsMovingRight => horizontalMovement > 0f;
	private bool IsMovingLeft => horizontalMovement < 0f;
	private bool CanFart => isFartingEnabled && availableFart >= fartUsageRange.y;

	public override void Flip() => body.Flip();

	public override bool Jump(float height)
	{
		if (!base.Jump(height))
			return false;

		animator.SetTrigger("Jump");

		return true;
	}

	public override void ApplyKnockback(Vector2 knockback, Vector2 direction)
	{
		if (!IsFarting)
			base.ApplyKnockback(knockback, direction);
	}

	public void SetFart(IFart newFart)
	{
		if (newFart == null) return;

		var fartInstance = Instantiate(newFart as MonoBehaviour, fartPoint.position, fartPoint.rotation) as MonoBehaviour;

		fartInstance.name = newFart.FartName;
		fartInstance.transform.parent = body;

		(currentFart as MonoBehaviour)?.DestroyGameObject();
		currentFart = fartInstance as IFart;
	}

	public void DisableInput()
	{
		isInputEnabled = false;
		ResetInput();
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();

		availableFart = maxAvailableFart;
	}

	private void OnEnable()
	{
		playerActions = PlayerActions.CreateWithDefaultBindings();
	}

	private void Start()
	{
		SetFart(startingFart as IFart);

		Player.Instance.Triggers.CarrotTriggered += CollectCarrot;
		Player.Instance.Triggers.FlagpoleTriggered += ActivateLevelFlagpole;
	}

	private void OnDisable()
	{
		playerActions.Destroy();
	}

	private void OnDestroy()
	{
		Player.Instance.Triggers.CarrotTriggered -= CollectCarrot;
		Player.Instance.Triggers.FlagpoleTriggered -= ActivateLevelFlagpole;
	}

	private void Update()
	{
		GetInput();
		ApplyAnimation();

		if (IsGrounded && !WasGrounded)
			PlayLandingSound();
	}

	private void LateUpdate()
	{
		if (Player.Instance.Health.IsDead)
			return;

		GetMovement();
		UpdateFartTrajectory();
		ApplyMovement();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (isFarting && fartingTime > 0.05f && CollisionLayers.ContainsLayer(other))
			StopFart(!IsGrounded);
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		OnTriggerEnter2D(other);
	}

	private void GetInput()
	{
		if (!isInputEnabled)
			return;

		horizontalMovement = playerActions.Move.Value;
		willJump = playerActions.Jump.WasPressed && IsGrounded;

		isFartCharging = playerActions.Fart.IsPressed && CanFart;

		if (isFartCharging)
		{
			var rawFartMagnitude = playerActions.Fart.Value.magnitude;

			if (rawFartMagnitude <= fartDeadZone)
			{
				isFartCharging = false;
				fartDirection = Vector2.zero;
				fartPower = 0f;
			}
			else
			{
				fartDirection = playerActions.Fart.Value.normalized;
				fartPower = Mathf.InverseLerp(fartDeadZone, 1f, rawFartMagnitude);
			}
		}

		if (!isFartCharging)
			willFart = playerActions.Fart.WasReleased && CanFart;
	}

	private void UpdateFartTrajectory()
	{
		if (IsFartCharging) currentFart.DrawTrajectory(FartPower, FartDirection, gravity, CenterPoint);
		else currentFart.ClearTrajectory();
	}

	private void ApplyAnimation()
	{
		animator.SetBool("Walking",  horizontalMovement != 0f && !isFarting);
		animator.SetBool("Grounded", IsGrounded);
		animator.SetBool("Falling", velocity.y < 0f);
	}

	private void GetMovement()
	{
		if (willFart) Fart(fartDirection, fartPower);

		if (!isFarting)
		{
			if (IsMovingRight && !IsFacingRight ||
					(IsMovingLeft && IsFacingRight))
				Flip();

			if (willJump) Jump(jumpHeight);

			if (!isFartingEnabled && IsGrounded) isFartingEnabled = true;

			availableFart = Mathf.Min(availableFart + (Time.deltaTime * fartRechargePerSecond), maxAvailableFart);
		}
	}

	private void ApplyMovement()
	{
		if (isFarting)
		{
			body.CorrectScaleForRotation(velocity.DirectionToRotation2D());
			fartingTime += Time.deltaTime;
		}
		else
		{
			float smoothedMovement = IsGrounded ? groundDamping : airDamping;

			velocity.x = Mathf.Lerp(velocity.x,
															horizontalMovement * moveSpeed,
															smoothedMovement * Time.deltaTime);
		}

		velocity.y += gravity * Time.deltaTime;
		controller.move(velocity * Time.deltaTime);
		velocity = controller.velocity;

		if (IsGrounded)
		{
			velocity.y = 0f;
			LastGroundedPosition = transform.position;
		}
	}

	private void Fart(Vector2 fartDirection, float fartPower)
	{
		if (IsFarting || FartDirection == Vector2.zero || FartPower <= 0) return;

		isFarting = true;
		isFartingEnabled = false;

		fartingTime = 0f;
		availableFart = Mathf.Max(0f, availableFart - CalculateFartUsage(fartPower));
		velocity = fartDirection * currentFart.CalculateSpeed(FartPower);

		currentFart.StartFart(FartPower, FartDirection);
	}

	private void StopFart(bool killXVelocity = true)
	{
		if (!IsFarting) return;

		isFarting = false;

		currentFart.StopFart();
		ResetOrientation();

		if (killXVelocity) velocity.x = 0f;

		velocity.y = 0f;
	}

	private float CalculateFartUsage(float fartPower) => Extensions.ConvertRange(fartPower, 0f, 1f, fartUsageRange.x, fartUsageRange.y);

	private void CollectCarrot(Carrot carrot)
	{
		if (carrot == null) return;

		carrot.Collect();
		availableFart = Mathf.Min(availableFart + (maxAvailableFart * carrotFartRechargePercent), maxAvailableFart);
	}

	private void ActivateLevelFlagpole(Flagpole flagpole)
	{
		if (flagpole.Activated) return;

		flagpole.Activate();
		DisableInput();
		Wait.ForSeconds(1.2f, GameMenu.Instance.ShowGameOver);
	}

	private void ResetOrientation()
	{
		float zRotation = body.rotation.eulerAngles.z;
		bool flipX = zRotation > 90f && zRotation < 270f;
		body.localScale = new Vector3(flipX ? -1f : 1f, 1f, 1f);
		body.rotation = Quaternion.identity;
	}

	private void ResetInput()
	{
		horizontalMovement = 0f;
		willJump = false;
		isFartCharging = false;
		StopFart(true);
		UpdateFartTrajectory();
	}

	private void PlayWalkingSound(int rightStep)
	{
		if (!IsFarting && IsGrounded)
			SoundManager.PlayCappedSFXFromGroup(rightStep == 1 ? SfxGroups.WalkingGrassRight 
				                                                 : SfxGroups.WalkingGrassLeft);
	}

	private void PlayLandingSound()
	{
		SoundManager.PlayCappedSFXFromGroup(SfxGroups.LandingGrass);
	}
}
