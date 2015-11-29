namespace BadTummyBunny
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

    public PlayerHealthContainersChangedMessage(int healthContainers)
    {
      HealthContainers = healthContainers;
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

  public class PlayerEnemyTriggeredMessage : IMessage
  {
    public IEnemy Enemy { get; }

    public PlayerEnemyTriggeredMessage(IEnemy enemy)
    {
      Enemy = enemy;
    }
  }

  public class PlayerCoinTriggeredMessage : IMessage
  {
    public Coin Coin { get; }

    public PlayerCoinTriggeredMessage(Coin coin)
    {
      Coin = coin;
    }
  }

  public class PlayerCarrotTriggeredMessage : IMessage
  {
    public Carrot Carrot { get; }

    public PlayerCarrotTriggeredMessage(Carrot carrot)
    {
      Carrot = carrot;
    }
  }

  public class PlayerFlagpoleTriggeredMessage : IMessage
  {
    public Flagpole Flagpole { get; }

    public PlayerFlagpoleTriggeredMessage(Flagpole flagpole)
    {
      Flagpole = flagpole;
    }
  }

  public class PlayerRespawnPointTriggeredMessage : IMessage
  {
    public RespawnPoint RespawnPoint { get; }

    public PlayerRespawnPointTriggeredMessage(RespawnPoint respawnPoint)
    {
      RespawnPoint = respawnPoint;
    }
  }

  public class PlayerKillzoneTriggeredMessage : IMessage { }
}