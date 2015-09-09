using System.Collections;
using UnityEngine;

[AddComponentMenu("Player/Control")]
public sealed class PlayerControl : MonoBehaviour
{
	#region Inspector Fields
	[Header("Movement")]
	public float gravity = -60f;
	public float walkSpeed = 10f;
	public float jumpHeight = 5f;

	public float groundDamping = 10f;
	public float airDamping = 5f;

	[Header("Farting")]
	public MonoBehaviour startingFart = null;
	public Transform fartPoint = null;
	public float maxAvailableFart = 10f;
	public float fartRechargePerSecond = 1f;
	[Range(0f, 1f)]
	public float carrotFartRechargePercent = 0.25f;
	public float fartDeadZone = 0.2f;
	public Vector2 fartUsageRange;
	public Vector2 fartSpeedRange;

	[Header("Components")]
	[SerializeField]
	private Transform body = null;
	#endregion

	#region Internal Fields
	private Vector3 velocity = Vector3.zero;
	private Vector3 lastGroundedPosition;
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
	private CharacterController2D controller;
	private Animator animator;
	#endregion

	#region Public Properties
	public static PlayerControl Instance { get; private set; }

	public bool IsFarting
	{ get { return isFarting; } }

	public bool IsFartCharging
	{ get { return isFartCharging; } }

	public bool WillFart
	{ get { return willFart; } }

	public Vector2 FartDirection
	{ get { return fartDirection; } }

	public float FartPower
	{ get { return fartPower; } }

	public float AvailableFartPercent
	{ get { return Mathf.Clamp01(availableFart / maxAvailableFart); } }

	public Vector3 Velocity
	{ get { return velocity; } }

	public Vector3 LastGroundedPosition
	{ get { return lastGroundedPosition; } }

	public Vector2 Direction
	{ get { return velocity.normalized; } }

	public bool IsGrounded
	{ get { return controller.isGrounded; } }

	public bool WasGrounded
	{ get { return controller.wasGroundedLastFrame; } }

	public LayerMask CollisionLayers
	{ get { return controller.platformMask; } }
	#endregion

	#region Internal Properties
	private bool IsMovingRight
	{ get { return horizontalMovement > 0f; } }

	private bool IsMovingLeft
	{ get { return horizontalMovement < 0f; } }

	private bool IsFacingRight
	{ get { return body.localScale.x > 0f; } }

	private Vector3 CenterPoint
	{ get { return collider2D.bounds.center; } }

	private bool CanFart
	{ get { return isFartingEnabled && availableFart >= fartUsageRange.y; } }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		controller = GetComponent<CharacterController2D>();
		animator = GetComponent<Animator>();

		availableFart = maxAvailableFart;
	}

	private void OnEnable()
	{
		playerActions = PlayerActions.CreateWithDefaultBindings();
	}

	private void Start()
	{
		SetFart(startingFart);

		PlayerTriggers.Instance.CarrotTriggered += CollectCarrot;
		PlayerTriggers.Instance.FlagpoleTriggered += ActivateLevelFlagpole;
	}

	private void OnDisable()
	{
		playerActions.Destroy();
	}

	private void OnDestroy()
	{
		PlayerTriggers.Instance.CarrotTriggered -= CollectCarrot;
		PlayerTriggers.Instance.FlagpoleTriggered -= ActivateLevelFlagpole;
	}

	private void Update()
	{
		GetInput();
		ApplyAnimation();

		if (IsGrounded && !WasGrounded) PlayLandingSound();
	}

	private void LateUpdate()
	{
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
	#endregion

	#region Internal Update Methods
	private void GetInput()
	{
		if (!isInputEnabled) return;

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

		if (!isFartCharging) willFart = playerActions.Fart.WasReleased && CanFart;
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
			if (IsMovingRight && !IsFacingRight) body.Flip();
			else if (IsMovingLeft && IsFacingRight) body.Flip();

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
															horizontalMovement * walkSpeed,
															smoothedMovement * Time.deltaTime);
		}

		velocity.y += gravity * Time.deltaTime;
		controller.move(velocity * Time.deltaTime);
		velocity = controller.velocity;

		if (IsGrounded)
		{
			velocity.y = 0f;
			lastGroundedPosition = transform.position;
		}
	}
	#endregion

	#region Internal Helper Methods
	private void Jump(float height)
	{
		if (height <= 0f) return;

		velocity.y = Mathf.Sqrt(2f * height * -gravity);
		animator.SetTrigger("Jump");
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

	private float CalculateFartUsage(float fartPower)
	{
		return Extensions.ConvertRange(fartPower,
																	 0f, 1f,
																	 fartUsageRange.x, fartUsageRange.y);
	}

	private void CollectCarrot(Carrot carrot)
	{
		if (carrot == null) return;

		carrot.Collect();
		PlayerHealth.Instance.Health += PlayerHealth.Instance.carrotHealthRecharge;
		availableFart = Mathf.Min(availableFart + (maxAvailableFart * carrotFartRechargePercent), maxAvailableFart);
	}

	private void ActivateLevelFlagpole(Flagpole flagpole)
	{
		if (flagpole.Activated) return;

		flagpole.Activate();
		DisableInput();
		StartCoroutine(GameMenu.Instance.ShowGameOver(1.2f));
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
		StopFart(true);
	}
	#endregion

	#region Internal Audio Methods
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
	#endregion

	#region Public Methods
	public void SetFart(MonoBehaviour newFart)
	{
		if (newFart == null) return;

		currentFart = Instantiate(newFart as MonoBehaviour, fartPoint.position, fartPoint.rotation) as IFart;
	}

	public IEnumerator ApplyKnockback(Vector2 knockback, float knockbackDirection)
	{
		yield return new WaitForSeconds(0.1f);

		velocity.x = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(knockback.x, 2) * -gravity)) * knockbackDirection;

		if (IsGrounded)
			velocity.y = Mathf.Sqrt(knockback.y * -gravity);

		controller.move(velocity * Time.deltaTime);
		velocity = controller.velocity;
	}

	public void DisableInput()
	{
		isInputEnabled = false;
		ResetInput();
	}
	#endregion
}
