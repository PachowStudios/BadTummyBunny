using UnityEngine.Assertions;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class PlayerScoreService : IScoreKeeper,
    IHandles<PlayerCoinCollectedMessage>
  {
    private int coins;

    [Inject] private IEventAggregator EventAggregator { get; set; }

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
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(PlayerCoinCollectedMessage message)
      => AddCoins(message.Value);

    public void AddCoins(int coinsToAdd)
    {
      Assert.IsTrue(coinsToAdd > 0);

      Coins += coinsToAdd;
    }

    public void RemoveCoins(int coinsToRemove)
    {
      Assert.IsTrue(coinsToRemove > 0);
      Assert.IsTrue(Coins - coinsToRemove >= 0);

      Coins -= coinsToRemove;
    }

    private void RaiseCoinsChanged()
      => EventAggregator.Publish(new PlayerCoinsChangedMessage(Coins));
  }
}