namespace PachowStudios.BadTummyBunny.Enemies.AI
{
  partial class PatrolAI
  {
    private class PatrolState : FiniteState<PatrolAI>
    {
      public PatrolState(FiniteStateMachine<PatrolAI> stateMachine, PatrolAI context)
        : base(stateMachine, context) { }

      public override void Enter()
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
}
