using UnityEngine;

[AddComponentMenu("Enemy/AI/Follow AI")]
public class FollowAI : BaseEnemyAI
{
	[Header("Follow AI")]
	[SerializeField]
	protected Vector2 followSpeedRange = new Vector2(2.5f, 3.5f);
	[SerializeField]
	protected float followRange = 5f;
	[SerializeField]
	protected float followBuffer = 0.75f;
	[SerializeField]
	protected float attackRange = 1f;
	[SerializeField]
	protected float attackJumpHeight = 0.5f;
	[SerializeField]
	protected float cooldownTime = 1f;

	private float defaultMoveSpeed;
	private float followSpeed;
	private bool isAttacking = false;
	private float cooldownTimer = 0f;

	protected virtual void Awake()
	{
		defaultMoveSpeed = moveSpeed;
		followSpeed = followSpeedRange.RandomRange();
	}

	protected override void UpdateAI()
	{
		Walk();
		CheckAttack();
		ApplyAnimation();
	}

	protected virtual void ApplyAnimation()
	{
		animator.SetBool("Walking", horizontalMovement != 0f);
		animator.SetBool("Grounded", IsGrounded);
		animator.SetBool("Falling", velocity.y < 0f);
	}

	private void Walk()
	{
		moveSpeed = defaultMoveSpeed;

		if (isAttacking && velocity.y < 0f && IsGrounded)
			isAttacking = false;

		if (horizontalMovement == 0f)
			horizontalMovement = Extensions.RandomSign();

		if (RelativePlayerLastGrounded != 0f)
		{
			CheckAtWall(true);
			CheckAtLedge(true);
		}
		else if (!CheckAtLedge())
		{
			if (RelativePlayerHeight < 0.5f && IsPlayerVisible(followRange))
			{
				FollowPlayer(followBuffer);
				moveSpeed = followSpeed;
			}
			else
			{
				CheckAtWall(true);
				CheckAtLedge(!isAttacking);
			}
		}
		else if (Player.Instance.Movement.IsGrounded)
			CheckAtLedge(!isAttacking);
		else
			horizontalMovement = 0f;
	}

	private void CheckAttack()
	{
		cooldownTimer += Time.deltaTime;

		if (cooldownTimer >= cooldownTime && IsPlayerInRange(0f, attackRange))
		{
			Attack();
			cooldownTimer = 0f;
		}
	}

	private void Attack()
	{
		isAttacking = true;
		horizontalMovement = IsPlayerOnRight ? 1f : -1f;
		Jump(attackJumpHeight);
	}
}
