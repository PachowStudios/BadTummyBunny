using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class PlayerCollidedMessage : IMessage
  {
    public Collider2D Collider { get; }

    public PlayerCollidedMessage(Collider2D collider)
    {
      Collider = collider;
    }
  }

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
}