namespace PachowStudios.BadTummyBunny.AI.Patrol
{
  public class PatrolState : FiniteState<PatrolAI>
  {
    public PatrolState(FiniteStateMachine<PatrolAI> stateMachine, PatrolAI context)
      : base(stateMachine, context) { }

    public override void Begin()
    {
      if (Context.HorizontalMovement == 0)
        Context.HorizontalMovement = MathHelper.RandomSign();
    }

    public override void Reason()
    {
      if (Context.CanFollowPlayer)
        StateMachine.GoTo<FollowState>();
    }

    public override void Tick(float deltaTime)
    {
      if (Context.IsAtWall || Context.IsAtLedge)
        Context.HorizontalMovement *= -1;
    }
  }
}
