using System.Diagnostics;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player/Triggers")]
  public sealed class PlayerTriggers : MonoBehaviour
  {
    [Inject]
    private IEventAggregator EventAggregator { get; set; }

    [Conditional("UNITY_EDITOR")]
    private void Update() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
          EventAggregator.Publish(new PlayerKillzoneTriggeredMessage());
          break;
      }
    }
  }
}
