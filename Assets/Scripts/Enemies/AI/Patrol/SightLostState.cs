namespace AI.Patrol
{
	public class SightLostState : FiniteState<PatrolAI>
	{
		private float waitTime;
		private float waitTimer;
		private bool flipped;

		public override void Begin()
		{
			Context.HorizontalMovement = 0;
			waitTime = Context.SightLostWaitTimeRange.RandomRange();
			waitTimer = waitTime;
			flipped = false;
		}

		public override void Reason()
		{
			if (Context.CanFollowPlayer)
				StateMachine.GoToState<FollowState>();

			if (waitTimer <= 0f)
			{
				Context.Flip();
				StateMachine.GoToState<PatrolState>();
			}
		}

		public override void Update(float deltaTime)
		{
			waitTimer -= deltaTime;

			if (waitTimer < waitTime / 2f && 
					!flipped                  &&
					!Context.IsAtLedge        &&
					!Context.IsAtWall)
			{
				Context.Flip();
				flipped = true;
			}
		}

		public override void End()
			=> Context.HorizontalMovement = Context.FacingDirection.x.RoundToInt();
	}
}