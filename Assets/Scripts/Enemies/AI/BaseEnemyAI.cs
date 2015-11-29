using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  public abstract class BaseEnemyAI : EnemyMovement
  {
    [Header("Attacking")]
    [SerializeField] private Vector2 knockback = new Vector2(2f, 1f);

    [Header("AI Settings")]
    [SerializeField] private LayerMask blockVisibilityLayers = default(LayerMask);

    [Header("AI Components")]
    [SerializeField] private Transform frontCheck = null;
    [SerializeField] private Transform ledgeCheck = null;

    [Inject(Tags.Player)]
    private IMovable PlayerMovement { get; set; }

    public bool IsAtWall
      => Physics2D.OverlapPoint(this.frontCheck.position, CollisionLayers) != null;

    public bool IsAtLedge
      => IsGrounded && Physics2D.OverlapPoint(this.ledgeCheck.position, CollisionLayers) == null;

    public bool IsPlayerOnRight
      => PlayerMovement.Position.x > ThisEnemy.Movement.Position.x;

    public float RelativePlayerLastGrounded
      => (ThisEnemy.Movement.LastGroundedPosition.y - PlayerMovement.LastGroundedPosition.y).RoundToHalf();

    public float RelativePlayerHeight
      => ThisEnemy.Movement.Position.y - PlayerMovement.Position.y;

    public virtual void FollowPlayer(float buffer = 1f)
    {
      if (transform.position.x + buffer < PlayerMovement.Position.x)
        HorizontalMovement = 1;
      else if (transform.position.x - buffer > PlayerMovement.Position.x)
        HorizontalMovement = -1;
      else
      {
        HorizontalMovement = 0;
        FacePlayer();
      }
    }

    public virtual void FacePlayer()
    {
      if (IsPlayerOnRight ^ IsFacingRight)
        transform.Flip();
    }

    public virtual bool IsPlayerInRange(float min = 0f, float max = Mathf.Infinity)
    {
      var distance = Mathf.Abs(PlayerMovement.Position.x - Position.x);

      return distance >= min && distance <= max;
    }

    public virtual bool IsPlayerInLineOfSight(float range = Mathf.Infinity, float maxAngle = 90f)
    {
      if (IsFacingRight ^ IsPlayerOnRight)
        return false;


      if (Vector3.Angle(FacingDirection, PlayerMovement.CenterPoint - CenterPoint) > maxAngle)
        return false;

      var linecast =
        Physics2D.Linecast(
          Collider.bounds.center,
          PlayerMovement.CenterPoint,
          this.blockVisibilityLayers);

      return linecast.collider == null && linecast.distance <= range;
    }
  }
}
