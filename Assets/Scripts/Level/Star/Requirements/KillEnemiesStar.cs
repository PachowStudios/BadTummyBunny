using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class KillEnemiesStar : BaseStarController,
    IHandles<PlayerKilledEnemyMessage>
  {
    [Inject] private KillEnemiesStarSettings Config { get; set; }

    private int enemiesKilled;

    private int EnemiesKilled
    {
      get { return this.enemiesKilled; }
      set
      {
        this.enemiesKilled = value;

        if (this.enemiesKilled >= Config.RequiredEnemies)
          Complete();
      }
    }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    protected override void OnCompleted()
      => EventAggregator.Unsubscribe(this);

    public void Handle(PlayerKilledEnemyMessage message)
      => EnemiesKilled++;
  }
}