using UnityEngine;

namespace PachowStudios.BadTummyBunny.AI.Patrol
{
  public class FollowState : FiniteState<PatrolAI>
  {
    private readonly float followSpeed;

    private float cooldownTimer;

    public FollowState(IFiniteStateMachine<PatrolAI> stateMachine, PatrolAI context)
      : base(stateMachine, context)
    {
      this.followSpeed = Context.FollowSpeedRange.RandomRange();
    }

    public override void Begin()
    {
      Context.MoveSpeedOverride = this.followSpeed;
      this.cooldownTimer = StateMachine.CameFrom<AttackState>() ? Context.CooldownTime : 0f;
    }

    public override void Reason()
    {
      if (Context.CanFollowPlayer)
      {
        if (Context.IsPlayerInRange(max: Context.AttackRange)
            && this.cooldownTimer <= 0f)
          StateMachine.GoTo<AttackState>();
      }
      else
        StateMachine.GoTo<SightLostState>();
    }

    public override void Tick(float deltaTime)
      => this.cooldownTimer -= deltaTime;

    public override void End()
      => Context.MoveSpeedOverride = null;
  }
}
