using System;
using UnityEngine;

[AddComponentMenu("Player/Score")]
public class PlayerScore : MonoBehaviour
{
	#region Events
	public event Action<int> CoinsChanged = delegate { };
	#endregion

	#region Internal Fields
	private int coins;
	#endregion

	#region Public Properties
	public static PlayerScore Instance { get; private set; }

	public int Coins
	{
		get { return coins; }
		set
		{
			coins = Mathf.Max(0, value);
			CoinsChanged(coins);
		}
	}
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		PlayerTriggers.Instance.CoinTriggered += CollectCoin;
	}

	private void OnDestroy()
	{
		PlayerTriggers.Instance.CoinTriggered -= CollectCoin;
	}
	#endregion

	#region Internal Helper Methods
	private void CollectCoin(Coin coin)
	{
		if (coin == null) return;

		Coins += coin.Value;
		coin.Collect();
	}
	#endregion
}
