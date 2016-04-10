using UnityEngine;

namespace PachowStudios.BadTummyBunny.Enemies.AI
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Enemies/AI/Patrol AI Settings")]
  public class PatrolAISettings : BaseEnemyAISettings
  {
    public Vector2 FollowSpeedRange = new Vector2(2.5f, 3.5f);
    public float VisibilityAngle = 45f;
    public float FollowRange = 5f;
    public float AttackRange = 1f;
    public float AttackJumpHeight = 0.5f;
    public float CooldownTime = 1f;
    public Vector2 SightLostWaitTimeRange = new Vector2(1f, 2.5f);
  }
}