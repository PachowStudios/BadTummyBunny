using UnityEngine;
using System.Collections;
using Vectrosity;

[AddComponentMenu("Player/Control")]
public sealed class PlayerControl : MonoBehaviour
{
	#region Fields
	public float gravity = -60f;
	public float walkSpeed = 10f;
	public float jumpHeight = 5f;

	public float fartMaxAvailableTime = 10f;
	[Range(0f, 1f)]
	public float fartRechargeRate = 0.5f;
	[Range(0f, 1f)]
	public float carrotFartRechargePercent = 0.25f;
	public float fartMaxChargeTime = 3f;
	public float fartMinDischarge = 0.5f;
	public Vector2 fartSpeedRange;

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

	public float groundDamping = 10f;
	public float airDamping = 5f;
	public float fartDamping = 10f;

	[Range(0f, 1f)]
	public float shortFartSoundPercentage = 0.25f;
	[Range(0f, 1f)]
	public float mediumFartSoundPercentage = 0.65f;

	[SerializeField]
	private Transform body;
	[SerializeField]
	private ParticleSystem fartParticles;

	private Vector3 velocity;
	private Vector3 lastGroundedPosition;
	private float horizontalMovement = 0f;
	private bool jump = false;
	private bool enableInput = true;

	private bool fart = false;
	private bool farted = false;
	private bool canFart = true;

	private bool fartCharging = false;
	private bool previousFartCharging = false;
	private float fartAvailableTime;
	private float fartChargeTime = 0f;

	private float fartSpeed = 0f;
	private float fartTime = 0f;
	private Vector2 fartDirection = Vector2.zero;

	private CharacterController2D controller;
	private Animator animator;
	private VectorLine trajectoryLine;
	#endregion

	#region Public Properties
	public static PlayerControl Instance { get; private set; }

	public bool Farting
	{ get { return farted; } }

	public float AvailableFartPercent
	{ get { return Mathf.Clamp(fartAvailableTime / fartMaxAvailableTime, 0f, 1f); } }

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

	private Vector3 MouseDirection
	{ get { return CenterPoint.LookAt2D(Camera.main.ScreenToWorldPoint(Input.mousePosition)) * Vector3.right; } }

	private Vector3 CenterPoint
	{ get { return collider2D.bounds.center; } }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		controller = GetComponent<CharacterController2D>();
		animator = GetComponent<Animator>();

		fartAvailableTime = fartMaxAvailableTime;

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

	private void Update()
	{
		GetInput();
		DrawFartTrajectory();
		ApplyAnimation();

		if (IsGrounded && !WasGrounded) PlayLandingSound();
	}

	private void LateUpdate()
	{
		GetMovement();
		ApplyMovement();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (farted && fartTime > 0.05f && CollisionLayers.ContainsLayer(other))
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

	private void OnDestroy()
	{
		VectorLine.Destroy(ref trajectoryLine);
	}
	#endregion

	#region Internal Update Methods
	private void GetInput()
	{
		if (enableInput)
		{
			horizontalMovement = Input.GetAxis("Horizontal");
			jump = jump || Input.GetButtonDown("Jump") && IsGrounded;

			previousFartCharging = fartCharging;
			fartCharging = Input.GetButton("Fart") && canFart;

			if (fartCharging && fartAvailableTime > 0f)
			{
				fartChargeTime = Mathf.Min(fartChargeTime + Time.deltaTime, fartMaxChargeTime);

				if (fartChargeTime < fartMaxChargeTime)
				{
					fartAvailableTime = Mathf.Max(fartAvailableTime - Time.deltaTime, 0f);

					if (!previousFartCharging)
						fartAvailableTime = Mathf.Max(fartAvailableTime - fartMinDischarge, 0f);
				}
			}
			else if (previousFartCharging)
			{
				Fart(fartChargeTime);
				fartChargeTime = 0f;
			}
			else
			{
				fartChargeTime = 0f;
				fartAvailableTime = Mathf.Min(fartAvailableTime + (Time.deltaTime * fartRechargeRate), fartMaxAvailableTime);
			}
		}
	}

	private void DrawFartTrajectory()
	{
		if (!fartCharging)
		{
			if (trajectoryLine.points3.Count != 0 && clearTrajectoryAfterFart) trajectoryLine.Resize(0);
		}
		else
		{
			var trajectoryPoints = CalculateFartTrajectory(fartChargeTime);

			if (trajectoryPoints == null) return;

			if (trajectoryLine.points3.Count == 0) trajectoryLine.Resize(trajectorySegments);

			trajectoryLine.SetColor(trajectoryGradient.Evaluate(fartChargeTime / fartMaxChargeTime));
			trajectoryLine.MakeSpline(trajectoryPoints);
		}

		trajectoryLine.Draw();
	}

	private void ApplyAnimation()
	{
		animator.SetBool("Walking",  horizontalMovement != 0f && !fart);
		animator.SetBool("Grounded", IsGrounded);
		animator.SetBool("Falling", velocity.y < 0f);
	}

	private void GetMovement()
	{
		if (!farted)
		{
			if (Right && !FacingRight)
				body.Flip();
			else if (Left && FacingRight)
				body.Flip();
		}

		if (!canFart && !Farting && IsGrounded)
			canFart = true;

		if (jump && IsGrounded)
		{
			Jump(jumpHeight);
			jump = false;
		}
	}

	private void ApplyMovement()
	{
		if (fart)
		{
			velocity = fartDirection * fartSpeed;
			fart = false;
		}

		if (farted)
		{
			body.CorrectScaleForRotation(velocity.DirectionToRotation2D());
			fartTime += Time.deltaTime;
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
		if (height > 0f)
		{
			velocity.y = Mathf.Sqrt(2f * height * -gravity);
			animator.SetTrigger("Jump");
		}
	}

	private float CalculateFartSpeed(float chargeTime)
	{
		return Extensions.ConvertRange(chargeTime, 
			                             0f, fartMaxChargeTime, 
																	 fartSpeedRange.x, fartSpeedRange.y);
	}

	private Vector3[] CalculateFartTrajectory(float chargeTime)
	{
		if (fartChargeTime <= 0f) return null;

		var trajectoryPoints = new Vector3[trajectorySegments];
		var trajectoryTimeStep = trajectoryPreviewTime / trajectorySegments;
		var trajectoryDirection = MouseDirection;
		var trajectoryFartSpeed = CalculateFartSpeed(chargeTime);
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

	private void Fart(float chargeTime)
	{
		fartSpeed = CalculateFartSpeed(chargeTime);
		fartTime = 0f;
		fartDirection = MouseDirection;
		StartCoroutine(StartFartParticles());
		fart = farted = true;
		canFart = false;

		PlayFartSound(Mathf.Clamp(chargeTime / fartMaxChargeTime, 0f, 1f));
	}

	private void StopFart(bool killXVelocity = true)
	{
		fart = farted = false;
		velocity.x = killXVelocity ? 0f : velocity.x;
		velocity.y = 0f;
		fartParticles.Stop();
		ResetOrientation();
	}

	private IEnumerator StartFartParticles()
	{
		yield return new WaitForFixedUpdate();

		fartParticles.Play();
	}

	private void CollectCarrot(Carrot carrot)
	{
		if (carrot == null) return;

		carrot.Collect();
		PlayerHealth.Instance.Health += (PlayerHealth.Instance.carrotHealthRechargePercent * PlayerHealth.Instance.maxHealth);
		fartAvailableTime = Mathf.Min(fartAvailableTime + (fartMaxAvailableTime * carrotFartRechargePercent), fartMaxAvailableTime);
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

	#region Audio Methods
	private void PlayWalkingSound(int rightStep)
	{
		if (!fart && IsGrounded)
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
