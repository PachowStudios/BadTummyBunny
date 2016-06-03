using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Enemies/Enemy View")]
  public class EnemyView : CharacterView<Enemy>, IView<IEnemy>, IActivatable
  {
    [SerializeField] private EnemyType type = default(EnemyType);
    [SerializeField] private Transform frontCheck = null;
    [SerializeField] private Transform ledgeCheck = null;

    [InjectLocal] private IEventAggregator EventAggregator { get; set; }

    [Inject] public override Enemy Model { get; protected set; }

    IEnemy IView<IEnemy>.Model => Model;

    public bool IsActivated
    {
      get { return Movement.IsActivated; }
      set { Movement.IsActivated = value; }
    }

    public EnemyType Type => this.type;
    public Transform FrontCheck => this.frontCheck;
    public Transform LedgeCheck => this.ledgeCheck;

    private EnemyMovement Movement => Model.Movement;

    private bool IsFacingMovementDirection
      => Movement.HorizontalMovement >= 0 && IsFacingRight
         || Movement.HorizontalMovement <= 0 && !IsFacingRight;

    [PostInject]
    private void PostInject()
      => name = Model.Name;

    protected override void LateUpdate()
    {
      base.LateUpdate();

      if (!IsFacingMovementDirection)
        Flip();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == Tags.Killzone)
        EventAggregator.Publish(new CharacterKillzoneTriggeredMessage());
    }

    private void OnTriggerStay2D(Collider2D other)
      => OnTriggerEnter2D(other);
  }
}