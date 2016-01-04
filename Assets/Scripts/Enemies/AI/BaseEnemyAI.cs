using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseEnemyAI : EnemyMovement
  {
    [InstallerSettings]
    public class BaseAISettings : MovementSettings
    {
      public LayerMask BlockVisibilityLayers = default(LayerMask);
    }

    [InjectLocal] private BaseAISettings Config { get; set; }

    [Inject] private Player Player { get; set; }

    public bool IsAtWall
      => Physics2D.OverlapPoint(EnemyView.FrontCheck.position, CollisionLayers) != null;

    public bool IsAtLedge
      => IsGrounded
      && Physics2D.OverlapPoint(EnemyView.LedgeCheck.position, CollisionLayers) == null;

    public bool IsPlayerOnRight => Player.Movement.Position.x > Position.x;
    public float RelativePlayerHeight => Position.y - Player.Movement.Position.y;

    public float RelativePlayerLastGrounded
      => (LastGroundedPosition.y - Player.Movement.LastGroundedPosition.y).RoundToFraction(2);

    public virtual void FollowPlayer(float buffer = 1f)
    {
      if (View.Transform.position.x + buffer < Player.Movement.Position.x)
        HorizontalMovement = 1;
      else if (View.Transform.position.x - buffer > Player.Movement.Position.x)
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
        View.Transform.Flip();
    }

    public virtual bool IsPlayerInRange(float min = 0f, float max = Mathf.Infinity)
    {
      var distance = Mathf.Abs(Player.Movement.Position.x - Position.x);

      return distance >= min && distance <= max;
    }

    public virtual bool IsPlayerInLineOfSight(float range = Mathf.Infinity, float maxAngle = 90f)
    {
      if (IsFacingRight ^ IsPlayerOnRight)
        return false;


      if (Vector3.Angle(FacingDirection, Player.Movement.CenterPoint - CenterPoint) > maxAngle)
        return false;

      var linecast =
        Physics2D.Linecast(
          Collider.bounds.center,
          Player.Movement.CenterPoint,
          Config.BlockVisibilityLayers);

      return linecast.collider == null && linecast.distance <= range;
    }
  }
}
