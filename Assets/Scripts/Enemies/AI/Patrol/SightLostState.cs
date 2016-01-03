using UnityEngine;

namespace PachowStudios.BadTummyBunny.AI.Patrol
{
  public class SightLostState : FiniteState<PatrolAI>
  {
    private float waitTime;
    private float waitTimer;
    private bool flipped;

    public SightLostState(FiniteStateMachine<PatrolAI> stateMachine, PatrolAI context)
      : base(stateMachine, context) { }

    public override void Begin()
    {
      Context.HorizontalMovement = 0;
      this.waitTime = Context.SightLostWaitTimeRange.RandomRange();
      this.waitTimer = this.waitTime;
      this.flipped = false;
    }

    public override void Reason()
    {
      if (Context.CanFollowPlayer)
        StateMachine.GoTo<FollowState>();

      if (this.waitTimer > 0f)
        return;

      Context.Flip();
      StateMachine.GoTo<PatrolState>();
    }

    public override void Tick(float deltaTime)
    {
      this.waitTimer -= deltaTime;

      if (this.waitTimer >= this.waitTime / 2f
          || this.flipped
          || Context.IsAtLedge
          || Context.IsAtWall)
        return;

      Context.Flip();
      this.flipped = true;
    }

    public override void End()
      => Context.HorizontalMovement = Context.FacingDirection.x.RoundToInt();
  }
}
