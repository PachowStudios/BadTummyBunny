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

    private void Awake()
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

    private void ApplyAnimation()
    {
      View.Animator.SetBool("Walking", HorizontalMovement != 0);
      View.Animator.SetBool("Grounded", IsGrounded);
      View.Animator.SetBool("Falling", Velocity.y < 0f);
    }
  }
}
