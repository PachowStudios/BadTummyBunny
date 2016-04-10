using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny.Enemies.AI
{
  public sealed partial class PatrolAI : BaseEnemyAI,
    IHandles<CharacterTookDamageMessage>
  {
    private PatrolAISettings Config { get; }
    private EnemyView View { get; }
    private FiniteStateMachine<PatrolAI> StateMachine { get; }
    private AnimationController AnimationController { get; }

    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

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

    [PostInject]
    private void PostInject()
      => LocalEventAggregator.Subscribe(this);

    protected override void InternalTick()
    {
      StateMachine.Tick(Time.deltaTime);
      AnimationController.Tick();
    }

    public void Handle(CharacterTookDamageMessage message) { }
    // BUG: Causes enemy to freeze up.
    //=> StateMachine.GoTo<FollowState>();
  }
}
