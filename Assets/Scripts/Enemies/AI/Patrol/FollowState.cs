using UnityEngine;

namespace PachowStudios.BadTummyBunny.Enemies.AI
{
  partial class PatrolAI
  {
    private class FollowState : FiniteState<PatrolAI>
    {
      private readonly float followSpeed;

      private float cooldownTimer;

      public FollowState(FiniteStateMachine<PatrolAI> stateMachine, PatrolAI context)
        : base(stateMachine, context)
      {
        this.followSpeed = Context.Config.FollowSpeedRange.RandomRange();
      }

      public override void Enter()
      {
        Context.FacePlayer();
        Context.MoveSpeedOverride = this.followSpeed;
        this.cooldownTimer = StateMachine.CameFrom<AttackState>() ? Context.Config.CooldownTime : 0f;
      }

      public override void Reason()
      {
        if (!Context.CanFollowPlayer)
          StateMachine.GoTo<SightLostState>();
        else if (Context.IsPlayerInRange(max: Context.Config.AttackRange)
                 && this.cooldownTimer <= 0f)
          StateMachine.GoTo<AttackState>();
      }

      public override void Tick(float deltaTime)
        => this.cooldownTimer -= deltaTime;

      public override void Leave()
        => Context.MoveSpeedOverride = null;
    }
  }
}
