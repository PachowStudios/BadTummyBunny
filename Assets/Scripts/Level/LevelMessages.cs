namespace PachowStudios.BadTummyBunny
{
  public class LevelCompletedMessage : IMessage
  {
    public Scene Scene { get; }

    public LevelCompletedMessage(Scene scene)
    {
      Scene = scene;
    }
  }
}