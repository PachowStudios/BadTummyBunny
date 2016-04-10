using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseEnemyAI : EnemyMovement
  {
    protected bool IsAtWall => Physics2D.OverlapPoint(View.FrontCheck.position, CollisionLayers) != null;
    protected bool IsAtLedge => IsGrounded && Physics2D.OverlapPoint(View.LedgeCheck.position, CollisionLayers) == null;
    protected bool IsFacingPlayer => IsPlayerOnRight == View.IsFacingRight;
    protected Vector2 FacingDirection => new Vector2(View.Transform.localScale.x, 0f);
    protected float RelativePlayerHeight => View.Position.y - Player.View.Position.y;
    protected float RelativePlayerLastGrounded
      => (LastGroundedPosition.y - Player.Movement.LastGroundedPosition.y).RoundToFraction(2);

    private BaseEnemyAISettings Config { get; }
    private EnemyView View { get; }

    [Inject] private Player Player { get; set; }

    private bool IsPlayerOnRight => Player.View.Position.x > View.Position.x;

    protected BaseEnemyAI(BaseEnemyAISettings config, EnemyView view)
      : base(config, view)
    {
      Config = config;
      View = view;
    }

    protected virtual void FollowPlayer(float buffer = 1f)
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

    protected virtual void FacePlayer()
    {
      if (!IsFacingPlayer)
        View.Flip();
    }

    protected virtual bool IsPlayerInRange(float min = 0f, float max = Mathf.Infinity)
    {
      var distance = Mathf.Abs(Player.View.Position.x - View.Position.x);

      return distance >= min && distance <= max;
    }

    protected virtual bool IsPlayerInLineOfSight(float range = Mathf.Infinity, float maxAngle = 90f)
    {
      if (!IsFacingPlayer
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
