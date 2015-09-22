using UnityEngine;

public abstract class EnemyMovement : BaseMovable, IActivatable
{
	[Header("Movement")]
	[SerializeField]
	protected float groundDamping = 10f;
	[SerializeField]
	protected float airDamping = 5f;
	[SerializeField]
	private bool deactivatedOutsideCamera = true;

	[Header("Components")]
	[SerializeField]
	protected Animator animator = null;

	private ICharacter thisEnemy = null;
	private bool isActivated = false;

	public bool IsActivated => isActivated || !deactivatedOutsideCamera;
	public int HorizontalMovement { get; set; } = 0;

	protected ICharacter ThisEnemy => this.GetInterfaceIfNull(ref thisEnemy);
	protected bool IsMovingRight => HorizontalMovement > 0;
	protected bool IsMovingLeft => HorizontalMovement < 0;

	private void Update()
	{
		if (IsActivated)
			InternalUpdate();
	}

	protected virtual void InternalUpdate() { }

	protected virtual void LateUpdate()
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
		var smoothedMovement = IsGrounded ? groundDamping : airDamping;

		velocity.x = Mathf.Lerp(velocity.x,
														HorizontalMovement * (MoveSpeedOverride ?? MoveSpeed),
														smoothedMovement * Time.deltaTime);
		velocity.y += gravity * Time.deltaTime;
		controller.move(velocity * Time.deltaTime);
		velocity = controller.velocity;

		if (IsGrounded)
		{
			velocity.y = 0f;
			LastGroundedPosition = Position;
		}
	}

	public override bool Jump(float height)
	{
		if (!base.Jump(height))
			return false;

		animator.SetTrigger("Jump");

		return true;
	}

	public virtual void Activate()
		=> isActivated = true;

	public virtual void Deactivate()
		=> isActivated = false;
}