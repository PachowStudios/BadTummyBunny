namespace PachowStudios.BadTummyBunny
{
  public interface ISceneLoader
  {
    void ReloadScene();
    void LoadScene(Scene scene);
    void QuitGame();
  }
}