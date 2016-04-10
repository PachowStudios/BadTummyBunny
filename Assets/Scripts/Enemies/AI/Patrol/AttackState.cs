namespace PachowStudios.BadTummyBunny.Enemies.AI
{
  partial class PatrolAI
  {
    private class AttackState : FiniteState<PatrolAI>
    {
      public AttackState(FiniteStateMachine<PatrolAI> stateMachine, PatrolAI context)
        : base(stateMachine, context) { }

      public override void Enter()
        => Context.Jump(Context.Config.AttackJumpHeight);

      public override void Reason()
      {
        if (Context.IsGrounded || Context.IsFalling)
          StateMachine.GoTo<FollowState>();
      }
    }
  }
}
