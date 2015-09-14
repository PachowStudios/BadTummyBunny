using System;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Player/Triggers")]
public sealed class PlayerTriggers : MonoBehaviour
{
	public event Action<Coin> CoinTriggered;
	public event Action<Enemy> EnemyTriggered;
	public event Action KillzoneTriggered;
	public event Action<RespawnPoint> RespawnPointTriggered;
	public event Action<Carrot> CarrotTriggered;
	public event Action<Flagpole> FlagpoleTriggered;

	[Conditional("UNITY_EDITOR")]
	private void Update() { }

	private void OnTriggerEnter2D(Collider2D other)
	{
		switch (other.tag)
		{
			case Tags.Coin:         CoinTriggered?.Invoke(other.GetComponent<Coin>()); break;
			case Tags.Enemy:        EnemyTriggered?.Invoke(other.GetComponent<Enemy>()); break;
			case Tags.Killzone:     KillzoneTriggered?.Invoke(); break;
			case Tags.RespawnPoint: RespawnPointTriggered?.Invoke(other.GetComponent<RespawnPoint>()); break;
			case Tags.Carrot:       CarrotTriggered?.Invoke(other.GetComponent<Carrot>()); break;
			case Tags.Flagpole:     FlagpoleTriggered?.Invoke(other.GetComponent<Flagpole>()); break;
		}
	}
}
