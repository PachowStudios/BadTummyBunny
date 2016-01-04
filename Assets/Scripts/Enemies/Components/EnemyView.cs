using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Enemy")]
  public class EnemyView : BaseView<Enemy>
  {
    [SerializeField] private Transform frontCheck = null;
    [SerializeField] private Transform ledgeCheck = null;

    [InjectLocal] private IEventAggregator EventAggregator { get; set; }

    [InjectLocal] public override Enemy Model { get; protected set; }

    public Transform FrontCheck => this.frontCheck;
    public Transform LedgeCheck => this.ledgeCheck;

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == Tags.Killzone)
        EventAggregator.Publish(new CharacterKillzoneTriggeredMessage());
    }

    private void OnTriggerStay2D(Collider2D other)
      => OnTriggerEnter2D(other);
  }
}