namespace PachowStudios.BadTummyBunny
{
  public class PlayerHealthChangedMessage : IMessage
  {
    public int Health { get; }

    public PlayerHealthChangedMessage(int health)
    {
      Health = health;
    }
  }

  public class PlayerHealthContainersChangedMessage : IMessage
  {
    public int HealthContainers { get; }
    public int HealthPerContainer { get; }

    public PlayerHealthContainersChangedMessage(int healthContainers, int healthPerContainer)
    {
      HealthContainers = healthContainers;
      HealthPerContainer = healthPerContainer;
    }
  }

  public class PlayerEnemyCollidedMessage : IMessage
  {
    public IEnemy Enemy { get; }

    public PlayerEnemyCollidedMessage(IEnemy enemy)
    {
      Enemy = enemy;
    }
  }

  public class PlayerKilledEnemyMessage : IMessage
  {
    public IEnemy Enemy { get; }

    public PlayerKilledEnemyMessage(IEnemy enemy)
    {
      Enemy = enemy;
    }
  }

  public class PlayerCoinsChangedMessage : IMessage
  {
    public int Coins { get; }

    public PlayerCoinsChangedMessage(int coins)
    {
      Coins = coins;
    }
  }

  public class PlayerCoinCollectedMessage : IMessage
  {
    public Coin Coin { get; }
    public int Value { get; }

    public PlayerCoinCollectedMessage(Coin coin)
    {
      Coin = coin;
      Value = coin.Value;
    }
  }

  public class PlayerCarrotCollectedMessage : IMessage
  {
    public Carrot Carrot { get; }

    public PlayerCarrotCollectedMessage(Carrot carrot)
    {
      Carrot = carrot;
    }
  }

  public class PlayerFlagpoleActivatedMessage : IMessage
  {
    public Flagpole Flagpole { get; }

    public PlayerFlagpoleActivatedMessage(Flagpole flagpole)
    {
      Flagpole = flagpole;
    }
  }

  public class PlayerRespawnPointActivatedMessage : IMessage
  {
    public RespawnPoint RespawnPoint { get; }

    public PlayerRespawnPointActivatedMessage(RespawnPoint respawnPoint)
    {
      RespawnPoint = respawnPoint;
    }
  }

  public class PlayerDiedMessage : IMessage { }
}