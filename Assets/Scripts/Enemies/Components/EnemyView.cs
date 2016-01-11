using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Enemies/Enemy View")]
  public class EnemyView : BaseView<Enemy>
  {
    [SerializeField] private EnemyType type = EnemyType.Fox;
    [SerializeField] private Transform frontCheck = null;
    [SerializeField] private Transform ledgeCheck = null;

    [InjectLocal] public override Enemy Model { get; protected set; }
    [InjectLocal] private IEventAggregator EventAggregator { get; set; }

    public EnemyType Type => this.type;
    public Transform FrontCheck => this.frontCheck;
    public Transform LedgeCheck => this.ledgeCheck;

    [PostInject]
    private void Initialize()
      => name = Model.Name;

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == Tags.Killzone)
        EventAggregator.Publish(new CharacterKillzoneTriggeredMessage());
    }

    private void OnTriggerStay2D(Collider2D other)
      => OnTriggerEnter2D(other);
  }
}