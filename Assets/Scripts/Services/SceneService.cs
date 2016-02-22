using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PachowStudios.BadTummyBunny
{
  public class SceneService : ISceneLoader
  {
    public void ReloadScene()
      => LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);

    public void LoadScene(Scene scene)
      => LoadScene(scene.GetDescription(), LoadSceneMode.Single);

    public void QuitGame()
      => Application.Quit();

    private void LoadScene(string levelName, LoadSceneMode mode)
      => SceneManager.LoadSceneAsync(levelName, mode);
  }
}