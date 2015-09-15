using UnityEngine;

public abstract class EnemyMovement : BaseMovable
{
	[Header("Movement")]
	[SerializeField]
	protected float groundDamping = 10f;
	[SerializeField]
	protected float airDamping = 5f;

	[Header("Components")]
	[SerializeField]
	protected Animator animator = null;

	protected float horizontalMovement = 0f;

	private ICharacter thisEnemy = null;

	protected ICharacter ThisEnemy => this.GetInterfaceIfNull(ref thisEnemy);
	protected bool IsMovingRight => horizontalMovement > 0f;
	protected bool IsMovingLeft => horizontalMovement < 0f;
	protected bool IsFacingRight => transform.localScale.x > 0f;

	public override void Move(Vector3 velocity)
	{
		controller.move(velocity * Time.deltaTime);
		velocity = controller.velocity;
	}

	protected virtual void LateUpdate()
	{
		GetMovement();
		ApplyMovement();
	}

	protected virtual void GetMovement()
	{
		if (IsMovingRight && !IsFacingRight)
			transform.Flip();
		else if (IsMovingLeft && IsFacingRight)
			transform.Flip();
	}

	protected virtual void ApplyMovement()
	{
		var smoothedMovement = IsGrounded ? groundDamping : airDamping;

		velocity.x = Mathf.Lerp(velocity.x,
														horizontalMovement * MoveSpeed,
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

	protected override void Jump(float height)
	{
		base.Jump(height);
		animator.SetTrigger("Jump");
	}
}