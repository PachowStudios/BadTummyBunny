using System;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Player/Triggers")]
public class PlayerTriggers : MonoBehaviour
{
	public event Action<Coin> CoinTriggered = delegate { };
	public event Action<Enemy> EnemyTriggered = delegate { };
	public event Action KillzoneTriggered = delegate { };
	public event Action<RespawnPoint> RespawnPointTriggered = delegate { };
	public event Action<Carrot> CarrotTriggered = delegate { };
	public event Action<Flagpole> FlagpoleTriggered = delegate { };

	public static PlayerTriggers Instance { get; private set; }

	[Conditional("UNITY_EDITOR")]
	protected void Update() { }

	private void Awake()
	{
		Instance = this;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		switch (other.tag)
		{
			case Tags.Coin:         CoinTriggered(other.GetComponent<Coin>()); break;
			case Tags.Enemy:        EnemyTriggered(other.GetComponent<Enemy>()); break;
			case Tags.Killzone:     KillzoneTriggered(); break;
			case Tags.RespawnPoint: RespawnPointTriggered(other.GetComponent<RespawnPoint>()); break;
			case Tags.Carrot:       CarrotTriggered(other.GetComponent<Carrot>()); break;
			case Tags.Flagpole:     FlagpoleTriggered(other.GetComponent<Flagpole>()); break;
		}
	}
}
