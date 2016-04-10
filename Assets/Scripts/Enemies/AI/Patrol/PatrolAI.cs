using UnityEngine;

namespace PachowStudios.BadTummyBunny.Enemies.AI
{
  public sealed partial class PatrolAI : BaseEnemyAI
  {
    private PatrolAISettings Config { get; }
    private EnemyView View { get; }

    private FiniteStateMachine<PatrolAI> StateMachine { get; }
    private AnimationController AnimationController { get; }

    private bool CanFollowPlayer
      => IsPlayerInLineOfSight(Config.FollowRange, Config.VisibilityAngle)
      && RelativePlayerLastGrounded.Abs() < 0.025f
      && !IsAtLedge
      && !IsAtWall;

    public PatrolAI(PatrolAISettings config, EnemyView view)
      : base(config, view)
    {
      Config = config;
      View = view;
      StateMachine = new FiniteStateMachine<PatrolAI>(this)
        .Add<PatrolState>()
        .Add<FollowState>()
        .Add<SightLostState>()
        .Add<AttackState>();
      AnimationController = new AnimationController(View.Animator,
        new AnimationCondition("Walking", () => IsWalking),
        new AnimationCondition("Grounded", () => IsGrounded),
        new AnimationCondition("Falling", () => IsFalling));
    }

    protected override void InternalTick()
    {
      StateMachine.Tick(Time.deltaTime);
      AnimationController.Tick();
    }
  }
}
