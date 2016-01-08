namespace PachowStudios.BadTummyBunny
{
  public class FartEnemyTriggeredMessage : IMessage
  {
    public IEnemy Enemy { get; }

    public FartEnemyTriggeredMessage(IEnemy enemy)
    {
      Enemy = enemy;
    }
  }
}