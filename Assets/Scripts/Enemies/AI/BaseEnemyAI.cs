using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseEnemyAI<TConfig> : EnemyMovement<TConfig>
    where TConfig : BaseEnemyAISettings
  {
    [Inject] private Player Player { get; set; }

    public Vector2 FacingDirection => new Vector2(View.Transform.localScale.x, 0f);
    public bool IsAtWall => Physics2D.OverlapPoint(View.FrontCheck.position, CollisionLayers) != null;
    public bool IsAtLedge => IsGrounded && Physics2D.OverlapPoint(View.LedgeCheck.position, CollisionLayers) == null;

    private bool IsPlayerOnRight => Player.View.Position.x > View.Position.x;
    protected float RelativePlayerHeight => View.Position.y - Player.View.Position.y;
    protected float RelativePlayerLastGrounded
      => (LastGroundedPosition.y - Player.Movement.LastGroundedPosition.y).RoundToFraction(2);

    public virtual void FollowPlayer(float buffer = 1f)
    {
      if (View.Transform.position.x + buffer < Player.View.Position.x)
        HorizontalMovement = 1;
      else if (View.Transform.position.x - buffer > Player.View.Position.x)
        HorizontalMovement = -1;
      else
      {
        HorizontalMovement = 0;
        FacePlayer();
      }
    }

    public virtual void FacePlayer()
    {
      if (IsPlayerOnRight ^ View.IsFacingRight)
        View.Flip();
    }

    public virtual bool IsPlayerInRange(float min = 0f, float max = Mathf.Infinity)
    {
      var distance = Mathf.Abs(Player.View.Position.x - View.Position.x);

      return distance >= min && distance <= max;
    }

    public virtual bool IsPlayerInLineOfSight(float range = Mathf.Infinity, float maxAngle = 90f)
    {
      if (View.IsFacingRight ^ IsPlayerOnRight
          || FacingDirection.AngleTo(Player.View.CenterPoint - View.CenterPoint) > maxAngle)
        return false;

      var linecast = Physics2D.Linecast(
        View.CenterPoint,
        Player.View.CenterPoint,
        Config.BlockVisibilityLayers);

      return !linecast && linecast.distance <= range;
    }
  }
}
