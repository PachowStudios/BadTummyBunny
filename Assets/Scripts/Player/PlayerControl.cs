using UnityEngine;
using System.Collections;
using InControl;
using Vectrosity;

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
	public float maxAvailableFart = 10f;
	public float fartRechargePerSecond = 1f;
	[Range(0f, 1f)]
	public float carrotFartRechargePercent = 0.25f;
	public float fartDeadZone = 0.2f;
	public Vector2 fartUsageRange;
	public Vector2 fartSpeedRange;

	[Header("Trajectory")]
	public float trajectoryPreviewTime = 1f;
	public float trajectoryStartDistance = 1f;
	public float trajectoryWidth = 0.3f;
	[Range(8, 64)]
	public int trajectorySegments = 16;
	public bool clearTrajectoryAfterFart = true;
	public Material trajectoryMaterial;
	public Gradient trajectoryGradient;
	public string trajectorySortingLayer;
	public int trajectorySortingOrder;

	[Header("Sound Effects")]
	[Range(0f, 1f)]
	public float shortFartSoundPercentage = 0.25f;
	[Range(0f, 1f)]
	public float mediumFartSoundPercentage = 0.65f;

	[Header("Components")]
	[SerializeField]
	private Transform body;
	[SerializeField]
	private ParticleSystem fartParticles;
	#endregion

	#region Internal Fields
	private Vector3 velocity = Vector3.zero;
	private Vector3 lastGroundedPosition;
	private float horizontalMovement = 0f;
	private bool jump = false;
	private bool fart = false;
	private bool enableInput = true;

	private bool farting = false;
	private bool fartingEnabled = true;

	private bool fartCharging = false;
	private float availableFart = 0f;
	private float fartPower = 0f;
	private Vector2 fartDirection = Vector2.zero;
	private float fartingTime = 0f;

	private PlayerActions playerActions;
	private CharacterController2D controller;
	private Animator animator;
	private VectorLine trajectoryLine;
	#endregion

	#region Public Properties
	public static PlayerControl Instance { get; private set; }

	public bool Farting
	{ get { return farting; } }

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
	private bool Right
	{ get { return horizontalMovement > 0f; } }

	private bool Left
	{ get { return horizontalMovement < 0f; } }

	private bool FacingRight
	{ get { return body.localScale.x > 0f; } }

	private Vector3 CenterPoint
	{ get { return collider2D.bounds.center; } }

	private bool CanFart
	{ get { return fartingEnabled && availableFart >= fartUsageRange.y; } }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		controller = GetComponent<CharacterController2D>();
		animator = GetComponent<Animator>();

		availableFart = maxAvailableFart;

		VectorLine.SetCanvasCamera(Camera.main);
		VectorLine.canvas.planeDistance = 9;
		VectorLine.canvas.sortingLayerName = trajectorySortingLayer;
		VectorLine.canvas.sortingOrder = trajectorySortingOrder;

		trajectoryLine = new VectorLine("Trajectory",
																		new Vector3[trajectorySegments],
																		trajectoryMaterial,
																		Extensions.UnitsToPixels(trajectoryWidth),
																		LineType.Continuous,
																		Joins.Fill);
		trajectoryLine.textureScale = 1f;
	}

	private void OnEnable()
	{
		playerActions = PlayerActions.CreateWithDefaultBindings();
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
		DrawFartTrajectory();
		ApplyMovement();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (farting && fartingTime > 0.05f && CollisionLayers.ContainsLayer(other))
			StopFart(!IsGrounded);

		switch (other.tag)
		{
			case Tags.Carrot:   CollectCarrot(other.GetComponent<Carrot>()); break;
			case Tags.Flagpole: ActivateLevelFlagpole(other.GetComponent<Flagpole>()); break;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		OnTriggerEnter2D(other);
	}

	private void OnDisable()
	{
		playerActions.Destroy();
	}

	private void OnDestroy()
	{
		VectorLine.Destroy(ref trajectoryLine);
	}
	#endregion

	#region Internal Update Methods
	private void GetInput()
	{
		if (!enableInput) return;

		horizontalMovement = playerActions.Move.Value;
		jump = playerActions.Jump.WasPressed && IsGrounded;

		fartCharging = playerActions.Fart.IsPressed && CanFart;

		if (fartCharging)
		{
			var rawFartMagnitude = playerActions.Fart.Value.magnitude;

			if (rawFartMagnitude <= fartDeadZone)
			{
				fartCharging = false;
				fartDirection = Vector2.zero;
				fartPower = 0f;
			}
			else
			{
				fartDirection = playerActions.Fart.Value.normalized;
				fartPower = Mathf.InverseLerp(fartDeadZone, 1f, rawFartMagnitude);
			}
		}

		if (!fartCharging) fart = playerActions.Fart.WasReleased && CanFart;
	}

	private void DrawFartTrajectory()
	{
		if (fartCharging)
		{
			var trajectoryPoints = CalculateFartTrajectory(fartPower);

			if (trajectoryPoints != null)
			{
				trajectoryLine.SetColor(trajectoryGradient.Evaluate(fartPower));
				trajectoryLine.MakeSpline(trajectoryPoints);
			}
			else trajectoryLine.SetColor(Color.clear);
		}
		else if (clearTrajectoryAfterFart) trajectoryLine.SetColor(Color.clear);

		trajectoryLine.Draw();
	}

	private void ApplyAnimation()
	{
		animator.SetBool("Walking",  horizontalMovement != 0f && !farting);
		animator.SetBool("Grounded", IsGrounded);
		animator.SetBool("Falling", velocity.y < 0f);
	}

	private void GetMovement()
	{
		if (fart) Fart(fartDirection, fartPower);

		if (!farting)
		{
			if (Right && !FacingRight) body.Flip();
			else if (Left && FacingRight) body.Flip();

			if (jump) Jump(jumpHeight);

			if (!fartingEnabled && IsGrounded) fartingEnabled = true;

			availableFart = Mathf.Min(availableFart + (Time.deltaTime * fartRechargePerSecond), maxAvailableFart);
		}
	}

	private void ApplyMovement()
	{
		if (farting)
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
		if (fartDirection == Vector2.zero || fartPower <= 0) return;

		fartingTime = 0f;
		farting = true;
		fartingEnabled = false;

		availableFart = Mathf.Max(0f, availableFart - CalculateFartUsage(fartPower));
		velocity = fartDirection * CalculateFartSpeed(fartPower);

		StartCoroutine(StartFartParticles());
		PlayFartSound(Mathf.Clamp01(fartPower));
	}

	private void StopFart(bool killXVelocity = true)
	{
		farting = false;
		fartParticles.Stop();
		ResetOrientation();

		if (killXVelocity) velocity.x = 0f;

		velocity.y = 0f;
	}

	private IEnumerator StartFartParticles()
	{
		yield return new WaitForFixedUpdate();

		fartParticles.Play();
	}

	private float CalculateFartUsage(float fartPower)
	{
		return Extensions.ConvertRange(fartPower,
																	 0f, 1f,
																	 fartUsageRange.x, fartUsageRange.y);
	}

	private float CalculateFartSpeed(float fartPower)
	{
		return Extensions.ConvertRange(fartPower, 
			                             0f, 1f, 
																	 fartSpeedRange.x, fartSpeedRange.y);
	}

	private Vector3[] CalculateFartTrajectory(float fartPower)
	{
		if (fartPower <= 0) return null;

		var trajectoryPoints = new Vector3[trajectorySegments];
		var trajectoryTimeStep = trajectoryPreviewTime / trajectorySegments;
		var trajectoryDirection = fartDirection.ToVector3();
		var trajectoryFartSpeed = CalculateFartSpeed(fartPower);
		var trajectoryVelocity = trajectoryDirection * trajectoryFartSpeed;
		var trajectoryGravity = gravity * trajectoryTimeStep * 0.5f;
		var startingPosition = CenterPoint;
		var bufferDelta = trajectoryDirection * trajectoryStartDistance;

		bufferDelta.y += gravity * Mathf.Pow((trajectoryStartDistance / trajectoryFartSpeed), 2) * 0.5f;

		for (int i = 0; i < trajectorySegments; i++)
		{
			var trajectoryDelta = trajectoryVelocity;
			trajectoryDelta.y += trajectoryGravity * i;
			trajectoryDelta *= trajectoryTimeStep * i;

			if (bufferDelta.sqrMagnitude > trajectoryDelta.sqrMagnitude)
				trajectoryPoints[i] = startingPosition + bufferDelta;
			else
				trajectoryPoints[i] = startingPosition + trajectoryDelta;

			if (i > 0)
			{
				var linecast = Physics2D.Linecast(trajectoryPoints[i - 1], trajectoryPoints[i], CollisionLayers.value);

				if (linecast.collider != null)
				{
					for (int j = i - 1; j < trajectorySegments; j++)
						trajectoryPoints[j] = linecast.point;

					break;
				}
			}
		}

		return trajectoryPoints;
	}

	private Color[] CalculateTrajectoryColors(Gradient gradient)
	{
		var colors = new Color[trajectorySegments - 1];

		for (int i = 0; i < trajectorySegments - 1; i++)
			colors[i] = gradient.Evaluate(i / (float)trajectorySegments);

		return colors;
	}

	private void CollectCarrot(Carrot carrot)
	{
		if (carrot == null) return;

		carrot.Collect();
		PlayerHealth.Instance.Health += PlayerHealth.Instance.carrotHealthRechargePercent * PlayerHealth.Instance.maxHealth;
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
		jump = false;
		StopFart(true);
	}
	#endregion

	#region Internal Audio Methods
	private void PlayWalkingSound(int rightStep)
	{
		if (!Farting && IsGrounded)
			SoundManager.PlayCappedSFXFromGroup(rightStep == 1 ? SfxGroups.WalkingGrassRight 
				                                                 : SfxGroups.WalkingGrassLeft);
	}

	private void PlayLandingSound()
	{
		SoundManager.PlayCappedSFXFromGroup(SfxGroups.LandingGrass);
	}

	private void PlayFartSound(float chargePercentage)
	{
		string fartGroup;

		if (chargePercentage <= shortFartSoundPercentage) fartGroup = SfxGroups.FartsShort;
		else if (chargePercentage <= mediumFartSoundPercentage) fartGroup = SfxGroups.FartsMedium;
		else fartGroup = SfxGroups.FartsLong;

		SoundManager.PlayCappedSFXFromGroup(fartGroup);
	}
	#endregion

	#region Public Methods
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
		enableInput = false;
		ResetInput();
	}
	#endregion
}
