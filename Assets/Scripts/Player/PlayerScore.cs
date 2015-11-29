using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Player/Score")]
  public class PlayerScore : MonoBehaviour, IScoreKeeper,
    IHandles<PlayerCoinTriggeredMessage>
  {
    private int coins;

    [Inject]
    private IEventAggregator EventAggregator { get; set; }

    public int Coins
    {
      get { return this.coins; }
      private set
      {
        this.coins = value;
        EventAggregator.Publish(new PlayerCoinsChangedMessage(this.coins));
      }
    }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void AddCoins(int coinsToAdd)
    {
      Assert.IsTrue(coinsToAdd > 0);

      Coins += coinsToAdd;
    }

    public void RemoveCoins(int coinsToRemove)
    {
      Assert.IsTrue(coinsToRemove > 0);

      Coins -= coinsToRemove;
    }

    private void CollectCoin(Coin coin)
    {
      Assert.IsNotNull(coin);

      AddCoins(coin.Value);
      coin.Collect();
    }

    public void Handle(PlayerCoinTriggeredMessage message)
      => CollectCoin(message.Coin);
  }
}
