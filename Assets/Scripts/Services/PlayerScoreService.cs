using PachowStudios.Assertions;
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
      coinsToAdd.Should().BeGreaterThan(0);

      Coins += coinsToAdd;
    }

    public void RemoveCoins(int coinsToRemove)
    {
      coinsToRemove.Should().BeGreaterThan(0);
      Coins.Should().BeAtLeast(coinsToRemove, "because the player cannot have fewer than 0 coins.");

      Coins -= coinsToRemove;
    }

    private void RaiseCoinsChanged()
      => EventAggregator.Publish(new PlayerCoinsChangedMessage(Coins));
  }
}