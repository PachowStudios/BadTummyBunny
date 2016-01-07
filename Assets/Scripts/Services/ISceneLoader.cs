namespace PachowStudios.BadTummyBunny
{
  public interface ISceneLoader
  {
    bool IsLoadingScene { get; }

    void ReloadScene();
    void LoadScene(Scene scene);
  }
}