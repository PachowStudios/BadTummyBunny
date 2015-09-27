namespace AI.Patrol
{
  public class PatrolState : FiniteState<PatrolAI>
  {
    public override void Begin()
    {
      if (Context.HorizontalMovement == 0)
        Context.HorizontalMovement = Extensions.RandomSign();
    }

    public override void Reason()
    {
      if (Context.CanFollowPlayer)
        StateMachine.GoToState<FollowState>();
    }

    public override void Update(float deltaTime)
    {
      if (Context.IsAtWall || Context.IsAtLedge)
        Context.HorizontalMovement *= -1;
    }
  }
}
