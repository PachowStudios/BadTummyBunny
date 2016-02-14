using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class PlayerStatsService :
    IHandles<PlayerCoinCollectedMessage>,
    IHandles<PlayerCarrotCollectedMessage>,
    IHandles<PlayerKilledEnemyMessage>
  {
    public int CoinsCollected { get; private set; }
    public int CarrotsCollected { get; private set; }
    public int EnemiesKilled { get; private set; }

    [Inject] private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(PlayerCoinCollectedMessage message)
      => CoinsCollected += message.Value;

    public void Handle(PlayerCarrotCollectedMessage message)
      => CarrotsCollected++;

    public void Handle(PlayerKilledEnemyMessage message)
      => EnemiesKilled++;
  }
}