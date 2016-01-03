using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player")]
  public class PlayerView : BaseView<Player>
  {
    [SerializeField] private Transform body = null;
    [SerializeField] private Transform fartPoint = null;

    [Inject] public override Player Model { get; protected set; }

    [InjectLocal] private IEventAggregator EventAggregator { get; set; }

    public Transform Body => this.body;
    public Transform FartPoint => this.fartPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
      EventAggregator.Publish(new PlayerCollidedMessage(other));

      switch (other.tag)
      {
        case Tags.Enemy:
          EventAggregator.Publish(new PlayerEnemyTriggeredMessage(other.GetInterface<IEnemy>()));
          break;
        case Tags.Coin:
          EventAggregator.Publish(new PlayerCoinTriggeredMessage(other.GetComponent<Coin>()));
          break;
        case Tags.Carrot:
          EventAggregator.Publish(new PlayerCarrotTriggeredMessage(other.GetComponent<Carrot>()));
          break;
        case Tags.Flagpole:
          EventAggregator.Publish(new PlayerFlagpoleTriggeredMessage(other.GetComponent<Flagpole>()));
          break;
        case Tags.RespawnPoint:
          EventAggregator.Publish(new PlayerRespawnPointTriggeredMessage(other.GetComponent<RespawnPoint>()));
          break;
        case Tags.Killzone:
          EventAggregator.Publish(new CharacterKillzoneTriggeredMessage());
          break;
      }
    }

    private void OnTriggerStay2D(Collider2D other)
      => OnTriggerEnter2D(other);
  }
}