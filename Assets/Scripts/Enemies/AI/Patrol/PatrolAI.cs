using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny.AI.Patrol
{
  public sealed class PatrolAI : BaseEnemyAI<PatrolAISettings>
  {
    [InjectLocal] protected override PatrolAISettings Config { get; set; }

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
    private AnimationController AnimationController { get; set; }

    [PostInject]
    private void Initialize()
    {
      StateMachine = new FiniteStateMachine<PatrolAI>(this)
        .Add<PatrolState>()
        .Add<FollowState>()
        .Add<SightLostState>()
        .Add<AttackState>();
      AnimationController = new AnimationController(View.Animator,
        new AnimationCondition("Walking", () => HorizontalMovement != 0),
        new AnimationCondition("Grounded", () => IsGrounded),
        new AnimationCondition("Falling", () => Velocity.y < 0f));
    }

    protected override void InternalTick()
    {
      StateMachine.Tick(Time.deltaTime);
      AnimationController.Tick();
    }
  }
}
