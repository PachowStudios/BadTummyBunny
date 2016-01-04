using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny.AI.Patrol
{
  public class PatrolAI : BaseEnemyAI
  {
    [InstallerSettings]
    public class Settings : BaseAISettings
    {
      public Vector2 FollowSpeedRange = new Vector2(2.5f, 3.5f);
      public float VisibilityAngle = 45f;
      public float FollowRange = 5f;
      public float AttackRange = 1f;
      public float AttackJumpHeight = 0.5f;
      public float CooldownTime = 1f;
      public Vector2 SightLostWaitTimeRange = new Vector2(1f, 2.5f);
    }

    [InjectLocal] private Settings Config { get; set; }

    public Vector2 FollowSpeedRange => Config.FollowSpeedRange;
    public float AttackRange => Config.AttackRange;
    public float AttackJumpHeight => Config.AttackJumpHeight;
    public float CooldownTime => Config.CooldownTime;
    public Vector2 SightLostWaitTimeRange => Config.SightLostWaitTimeRange;

    public bool CanFollowPlayer
      => IsPlayerInLineOfSight(Config.FollowRange, Config.VisibilityAngle)
      && Math.Abs(RelativePlayerLastGrounded) < 0.025f
      && !IsAtLedge
      && !IsAtWall;

    private FiniteStateMachine<PatrolAI> StateMachine { get; set; }

    protected virtual void Awake()
      => StateMachine = new FiniteStateMachine<PatrolAI>(this)
        .Add<PatrolState>()
        .Add<FollowState>()
        .Add<SightLostState>()
        .Add<AttackState>();

    protected override void InternalTick()
    {
      StateMachine.Tick(Time.deltaTime);
      ApplyAnimation();
    }

    protected virtual void ApplyAnimation()
    {
      View.Animator.SetBool("Walking", HorizontalMovement != 0);
      View.Animator.SetBool("Grounded", IsGrounded);
      View.Animator.SetBool("Falling", Velocity.y < 0f);
    }
  }
}
