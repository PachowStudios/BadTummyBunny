namespace AI.Patrol
{
	public class FollowState : FiniteState<PatrolAI>
	{
		private float followSpeed;
		private float cooldownTimer;

		public override void OnInitialized() 
			=> followSpeed = Context.FollowSpeedRange.RandomRange();

		public override void Begin()
		{
			Context.MoveSpeedOverride = followSpeed;

			if (StateMachine.CameFromState<AttackState>())
				cooldownTimer = Context.CooldownTime;
			else
				cooldownTimer = 0f;
		}

		public override void Reason()
		{
			if (Context.CanFollowPlayer)
			{
				if (Context.IsPlayerInRange(max: Context.AttackRange) &&
						cooldownTimer <= 0f)
					StateMachine.GoToState<AttackState>();
			}
			else
				StateMachine.GoToState<SightLostState>();
		}

		public override void Update(float deltaTime)
			=> cooldownTimer -= deltaTime;

		public override void End()
			=> Context.MoveSpeedOverride = null;
	}
}