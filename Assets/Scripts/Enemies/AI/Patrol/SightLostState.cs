namespace BadTummyBunny.AI.Patrol
{
  public class SightLostState : FiniteState<PatrolAI>
  {
    private float waitTime;
    private float waitTimer;
    private bool flipped;

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
        StateMachine.GoToState<FollowState>();

      if (this.waitTimer <= 0f)
      {
        Context.Flip();
        StateMachine.GoToState<PatrolState>();
      }
    }

    public override void Update(float deltaTime)
    {
      this.waitTimer -= deltaTime;

      if (this.waitTimer < this.waitTime / 2f
          && !this.flipped
          && !Context.IsAtLedge
          && !Context.IsAtWall)
      {
        Context.Flip();
        this.flipped = true;
      }
    }

    public override void End()
      => Context.HorizontalMovement = Context.FacingDirection.x.RoundToInt();
  }
}
