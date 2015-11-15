public class LevelSelectedMessage : IMessage
{
  public WorldMapLevel Level { get; }

  public LevelSelectedMessage(WorldMapLevel level)
  {
    Level = level;
  }
}

public class LevelDeselectedMessage : IMessage
{
  public WorldMapLevel Level { get; }

  public LevelDeselectedMessage(WorldMapLevel level)
  {
    Level = level;
  }
}