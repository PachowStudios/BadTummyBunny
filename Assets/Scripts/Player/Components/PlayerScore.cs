using JetBrains.Annotations;
using UnityEngine.Assertions;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class PlayerScore : IScoreKeeper,
    IHandles<PlayerCoinTriggeredMessage>
  {
    private int coins;

    [Inject(BindingIds.Global)] private IEventAggregator EventAggregator { get; set; }
    [Inject] private IEventAggregator LocalEventAggregator { get; set; }

    public int Coins
    {
      get { return this.coins; }
      private set
      {
        this.coins = value;
        RaiseCoinsChanged();
      }
    }

    [PostInject]
    private void Initialize()
      => LocalEventAggregator.Subscribe(this);

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

    private void CollectCoin([NotNull] Coin coin)
    {
      AddCoins(coin.Value);
      coin.Collect();
    }

    private void RaiseCoinsChanged()
      => EventAggregator.Publish(new PlayerCoinsChangedMessage(Coins));

    public void Handle(PlayerCoinTriggeredMessage message)
      => CollectCoin(message.Coin);
  }
}
