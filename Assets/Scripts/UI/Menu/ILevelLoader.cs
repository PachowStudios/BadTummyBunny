namespace PachowStudios.BadTummyBunny
{
  public interface ILevelLoader
  {
    bool IsLoadingLevel { get; }

    void RetryLevel();
    void LoadLevel(Level level);
    void QuitGame();
  }
}