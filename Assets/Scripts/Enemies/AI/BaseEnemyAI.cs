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

	public bool IsAtWall 
		=> Physics2D.OverlapPoint(frontCheck.position, CollisionLayers) != null;

	public bool IsAtLedge 
		=> IsGrounded && Physics2D.OverlapPoint(ledgeCheck.position, CollisionLayers) == null;

	public bool IsPlayerOnRight 
		=> Player.Instance.Movement.Position.x > ThisEnemy.Movement.Position.x;

	public float RelativePlayerLastGrounded 
		=> (ThisEnemy.Movement.LastGroundedPosition.y - Player.Instance.Movement.LastGroundedPosition.y).RoundToHalf();

	public float RelativePlayerHeight 
		=> ThisEnemy.Movement.Position.y - Player.Instance.Movement.Position.y;

	public virtual void FollowPlayer(float buffer = 1f)
	{
		if (transform.position.x + buffer < Player.Instance.Movement.Position.x)
			HorizontalMovement = 1;
		else if (transform.position.x - buffer > Player.Instance.Movement.Position.x)
			HorizontalMovement = -1;
		else
		{
			HorizontalMovement = 0;
			FacePlayer();
		}
	}

	public virtual void FacePlayer()
	{
		if ((IsPlayerOnRight && !IsFacingRight) ||
				(!IsPlayerOnRight && IsFacingRight))
			transform.Flip();
	}

	public virtual bool IsPlayerInRange(float min = 0f, float max = Mathf.Infinity)
	{
		var distance = Mathf.Abs(Player.Instance.Movement.Position.x - Position.x);

		return distance >= min && distance <= max;
	}

	public virtual bool IsPlayerInLineOfSight(float range = Mathf.Infinity, float maxAngle = 90f)
	{
		if (IsFacingRight ^ IsPlayerOnRight)
			return false;

		var playerDirection = Player.Instance.Movement.CenterPoint - CenterPoint;

		if (Vector3.Angle(FacingDirection, playerDirection) > maxAngle)
			return false;

		var linecast = Physics2D.Linecast(collider2D.bounds.center,
																			Player.Instance.Movement.CenterPoint,
																			blockVisibilityLayers);

		return linecast.collider == null && linecast.distance <= range;
	}
}