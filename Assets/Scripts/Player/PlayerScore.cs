using System;
using UnityEngine;

[AddComponentMenu("Player/Score")]
public class PlayerScore : MonoBehaviour, IScoreKeeper
{
	public event Action<int> CoinsChanged;

	private int coins = 0;

	public int Coins
	{
		get { return coins; }
		private set
		{
			coins = value;
			CoinsChanged?.Invoke(coins);
		}
	}

	public void AddCoins(int coinsToAdd)
	{
		if (coinsToAdd <= 0)
			Debug.LogError($"{nameof(coinsToAdd)} cannot be 0 or less. Was {coinsToAdd}");

		Coins += coinsToAdd;
	}

	public void RemoveCoins(int coinsToRemove)
	{
		if (coinsToRemove >= 0)
			Debug.LogError($"{nameof(coinsToRemove)} cannot be 0 or greater. Was {coinsToRemove}");

		Coins -= coinsToRemove;
	}

	private void Start() => Player.Instance.Triggers.CoinTriggered += CollectCoin;

	private void OnDestroy() => Player.Instance.Triggers.CoinTriggered -= CollectCoin;

	private void CollectCoin(Coin coin)
	{
		if (coin == null)
			Debug.LogError($"{nameof(coin)} was null");

		AddCoins(coin.Value);
		coin.Collect();
	}
}
