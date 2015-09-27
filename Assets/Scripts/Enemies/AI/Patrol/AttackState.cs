namespace AI.Patrol
{
  public class AttackState : FiniteState<PatrolAI>
  {
    public override void Begin()
      => Context.Jump(Context.AttackJumpHeight);

    public override void Reason()
    {
      if (Context.IsGrounded || Context.IsFalling)
        StateMachine.GoToState<FollowState>();
    }
  }
}
