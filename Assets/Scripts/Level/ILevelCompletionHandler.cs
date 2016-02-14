namespace PachowStudios.BadTummyBunny
{
  public interface ILevelCompletionHandler
  {
    bool IsCompleted { get; }

    void CompleteLevel();
  }
}