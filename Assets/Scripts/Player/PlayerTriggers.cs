using System;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Player/Triggers")]
public sealed class PlayerTriggers : MonoBehaviour
{
  public event Action<IEnemy>       EnemyTriggered;
  public event Action<Coin>         CoinTriggered;
  public event Action<Carrot>       CarrotTriggered;
  public event Action<Flagpole>     FlagpoleTriggered;
  public event Action<RespawnPoint> RespawnPointTriggered;
  public event Action               KillzoneTriggered;

  [Conditional("UNITY_EDITOR")]
  private void Update() { }

  private void OnTriggerEnter2D(Collider2D other)
  {
    switch (other.tag)
    {
      case Tags.Enemy: EnemyTriggered?.Invoke(other.GetInterface<IEnemy>()); break;
      case Tags.Coin: CoinTriggered?.Invoke(other.GetComponent<Coin>()); break;
      case Tags.Carrot: CarrotTriggered?.Invoke(other.GetComponent<Carrot>()); break;
      case Tags.Flagpole: FlagpoleTriggered?.Invoke(other.GetComponent<Flagpole>()); break;
      case Tags.RespawnPoint: RespawnPointTriggered?.Invoke(other.GetComponent<RespawnPoint>()); break;
      case Tags.Killzone: KillzoneTriggered?.Invoke(); break;
    }
  }
}
