using UnityEngine;

public abstract class BaseEnemyAI : EnemyMovement
{
	[Header("Attacking")]
	[SerializeField]
	protected Vector2 knockback = new Vector2(2f, 1f);

	[Header("AI Settings")]
	[SerializeField]
	protected LayerMask blockVisibilityLayers = default(LayerMask);

	[Header("AI Components")]
	[SerializeField]
	protected Transform frontCheck = null;
	[SerializeField]
	protected Transform ledgeCheck = null;

	protected bool IsPlayerOnRight => Player.Instance.Movement.Position.x > ThisEnemy.Movement.Position.x;
	protected float RelativePlayerLastGrounded => (ThisEnemy.Movement.LastGroundedPosition.y - Player.Instance.Movement.LastGroundedPosition.y).RoundToTenth();
	protected float RelativePlayerHeight => ThisEnemy.Movement.Position.y - Player.Instance.Movement.Position.y;

	protected virtual void Update()
	{
		UpdateAI();
	}

	protected abstract void UpdateAI();

	protected virtual void FollowPlayer(float range)
	{
		if (transform.position.x + range < Player.Instance.Movement.Position.x)
			horizontalMovement = 1f;
		else if (transform.position.x - range > Player.Instance.Movement.Position.x)
			horizontalMovement = -1f;
		else
		{
			horizontalMovement = 0f;
			FacePlayer();
		}
	}

	protected virtual void FacePlayer()
	{
		if ((IsPlayerOnRight && !IsFacingRight) ||
				(!IsPlayerOnRight && IsFacingRight))
			transform.Flip();
	}

	protected virtual bool CheckAtWall(bool flip = false)
	{
		var collision = Physics2D.OverlapPoint(frontCheck.position, CollisionLayers);
		var isAtWall = collision != null;

		if (isAtWall && flip)
		{
			horizontalMovement *= -1f;

			if (horizontalMovement == 0f)
				horizontalMovement = Extensions.RandomSign();
		}

		return isAtWall;
	}

	protected virtual bool CheckAtLedge(bool flip = false)
	{
		if (!IsGrounded) return false;

		var collision = Physics2D.OverlapPoint(ledgeCheck.position, CollisionLayers);
		var isAtLedge = collision == null;

		if (isAtLedge && flip)
		{
			horizontalMovement *= -1f;

			if (horizontalMovement == 0f)
				horizontalMovement = Extensions.RandomSign();
		}

		return isAtLedge;
	}

	protected virtual bool IsPlayerInRange(float min, float max)
	{
		var startPoint = new Vector3(Position.x + (min * Direction.x), collider2D.bounds.center.y);
		var endPoint = startPoint + new Vector3((max - min) * Direction.x, 0f);
		var linecast = Physics2D.Linecast(startPoint, endPoint, LayerMask.GetMask(Tags.Player));

		return linecast.collider != null;
	}

	protected virtual bool IsPlayerVisible(float range = Mathf.Infinity)
	{
		var linecast = Physics2D.Linecast(collider2D.bounds.center,
																			Player.Instance.collider2D.bounds.center,
																			blockVisibilityLayers);

		return linecast.collider == null && linecast.distance <= range;
	}
}