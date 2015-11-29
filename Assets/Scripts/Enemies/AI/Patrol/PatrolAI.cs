using System;
using UnityEngine;

namespace BadTummyBunny.AI.Patrol
{
  [AddComponentMenu("Enemy/AI/Patrol AI")]
  public class PatrolAI : BaseEnemyAI
  {
    [Header("Patrol AI")]
    [SerializeField] private Vector2 followSpeedRange = new Vector2(2.5f, 3.5f);
    [SerializeField] private float visibilityAngle = 45f;
    [SerializeField] private float followRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackJumpHeight = 0.5f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private Vector2 sightLostWaitTimeRange = new Vector2(1f, 2.5f);

    public Vector2 FollowSpeedRange => this.followSpeedRange;
    public float AttackRange => this.attackRange;
    public float AttackJumpHeight => this.attackJumpHeight;
    public float CooldownTime => this.cooldownTime;
    public Vector2 SightLostWaitTimeRange => this.sightLostWaitTimeRange;
    public Animator Animator => this.animator;

    public bool CanFollowPlayer
      => IsPlayerInLineOfSight(this.followRange, this.visibilityAngle)
      && Math.Abs(RelativePlayerLastGrounded) < 0.025f
      && !IsAtLedge
      && !IsAtWall;

    protected IFiniteStateMachine<PatrolAI> StateMachine { get; private set; }

    protected virtual void Awake()
      => StateMachine = new FiniteStateMachine<PatrolAI>(this)
        .AddState<PatrolState>()
        .AddState<FollowState>()
        .AddState<SightLostState>()
        .AddState<AttackState>();

    protected override void InternalUpdate()
    {
      StateMachine.Update(Time.deltaTime);
      ApplyAnimation();
    }

    protected virtual void ApplyAnimation()
    {
      this.animator.SetBool("Walking", HorizontalMovement != 0);
      this.animator.SetBool("Grounded", IsGrounded);
      this.animator.SetBool("Falling", Velocity.y < 0f);
    }
  }
}
