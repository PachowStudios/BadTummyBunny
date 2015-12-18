namespace PachowStudios.BadTummyBunny.AI.Patrol
{
  public class AttackState : FiniteState<PatrolAI>
  {
    public AttackState(IFiniteStateMachine<PatrolAI> stateMachine, PatrolAI context)
      : base(stateMachine, context) { }

    public override void Begin()
      => Context.Jump(Context.AttackJumpHeight);

    public override void Reason()
    {
      if (Context.IsGrounded || Context.IsFalling)
        StateMachine.GoTo<FollowState>();
    }
  }
}
