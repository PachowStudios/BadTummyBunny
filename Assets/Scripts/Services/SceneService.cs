using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PachowStudios.BadTummyBunny
{
  public class SceneService : ISceneLoader
  {
    private AsyncOperation LoadingOperation { get; set; }

    public bool IsLoadingScene => !LoadingOperation?.isDone ?? false;

    public void ReloadScene()
      => LoadScene(SceneManager.GetActiveScene().name);

    public void LoadScene(Scene scene)
      => LoadScene(scene.GetDescription());

    public void QuitGame()
      => Application.Quit();

    private void LoadScene(string levelName)
    {
      if (!IsLoadingScene)
        LoadingOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
    }
  }
}