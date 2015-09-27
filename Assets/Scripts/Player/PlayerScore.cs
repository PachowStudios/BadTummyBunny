using System;
using UnityEngine;

[AddComponentMenu("Player/Score")]
public class PlayerScore : MonoBehaviour, IScoreKeeper
{
  public event Action<int> CoinsChanged;

  private int coins;

  public int Coins
  {
    get { return this.coins; }
    private set
    {
      this.coins = value;
      CoinsChanged?.Invoke(this.coins);
    }
  }

  public void AddCoins(int coinsToAdd)
  {
    Assert.IsGreaterThan(coinsToAdd, 0);

    Coins += coinsToAdd;
  }

  public void RemoveCoins(int coinsToRemove)
  {
    Assert.IsGreaterThan(coinsToRemove, 0);

    Coins -= coinsToRemove;
  }

  private void Start() => Player.Instance.Triggers.CoinTriggered += CollectCoin;

  private void OnDestroy() => Player.Instance.Triggers.CoinTriggered -= CollectCoin;

  private void CollectCoin(Coin coin)
  {
    Assert.IsNotNull(coin);

    AddCoins(coin.Value);
    coin.Collect();
  }
}
