namespace AI.Patrol
{
  public class FollowState : FiniteState<PatrolAI>
  {
    private float followSpeed;
    private float cooldownTimer;

    public override void OnInitialized()
      => this.followSpeed = Context.FollowSpeedRange.RandomRange();

    public override void Begin()
    {
      Context.MoveSpeedOverride = this.followSpeed;
      this.cooldownTimer = StateMachine.CameFromState<AttackState>() ? Context.CooldownTime : 0f;
    }

    public override void Reason()
    {
      if (Context.CanFollowPlayer)
      {
        if (Context.IsPlayerInRange(max: Context.AttackRange) && this.cooldownTimer <= 0f)
          StateMachine.GoToState<AttackState>();
      }
      else
        StateMachine.GoToState<SightLostState>();
    }

    public override void Update(float deltaTime)
      => this.cooldownTimer -= deltaTime;

    public override void End()
      => Context.MoveSpeedOverride = null;
  }
}
