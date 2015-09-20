using UnityEngine;

namespace AI.Patrol
{
	[AddComponentMenu("Enemy/AI/Patrol AI")]
	public class PatrolAI : BaseEnemyAI
	{
		[Header("Patrol AI")]
		[SerializeField]
		protected Vector2 followSpeedRange = new Vector2(2.5f, 3.5f);
		[SerializeField]
		protected float visibilityAngle = 45f;
		[SerializeField]
		protected float followRange = 5f;
		[SerializeField]
		protected float followBuffer = 0.75f;
		[SerializeField]
		protected float attackRange = 1f;
		[SerializeField]
		protected float attackJumpHeight = 0.5f;
		[SerializeField]
		protected float cooldownTime = 1f;
		[SerializeField]
		protected Vector2 sightLostWaitTimeRange = new Vector2(1f, 2.5f);

		public Vector2 FollowSpeedRange => followSpeedRange;
		public float AttackRange => attackRange;
		public float AttackJumpHeight => attackJumpHeight;
		public float CooldownTime => cooldownTime;
		public Vector2 SightLostWaitTimeRange => sightLostWaitTimeRange;
		public Animator Animator => animator;

		public bool CanFollowPlayer
			=> IsPlayerInLineOfSight(followRange, visibilityAngle) &&
				 RelativePlayerLastGrounded == 0f                    &&
				 !IsAtLedge                                          &&
				 !IsAtWall;

		protected IFiniteStateMachine<PatrolAI> StateMachine { get; private set; }

		protected virtual void Awake()
			=> StateMachine = new FiniteStateMachine<PatrolAI>(this)
														.AddState<PatrolState>()
														.AddState<FollowState>()
														.AddState<SightLostState>()
														.AddState<AttackState>();

		protected virtual void Update()
		{
			StateMachine.Update(Time.deltaTime);
			ApplyAnimation();
		}

		protected virtual void ApplyAnimation()
		{
			animator.SetBool("Walking", HorizontalMovement != 0);
			animator.SetBool("Grounded", IsGrounded);
			animator.SetBool("Falling", Velocity.y < 0f);
		}
	}
}
