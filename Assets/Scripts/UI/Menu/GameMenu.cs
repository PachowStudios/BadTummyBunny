using System;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class GameMenu : MonoBehaviour, IGameMenu, ILevelLoader
  {
    private CanvasGroup canvasGroupComponent;

    private AsyncOperation LoadingOperation { get; set; }

    private CanvasGroup CanvasGroup => this.GetComponentIfNull(ref this.canvasGroupComponent);

    public bool IsLoadingLevel => !(LoadingOperation?.isDone ?? true);

    public void ShowGameOverScreen()
      => SetGameOverScreenVisiblity(true);

    public void HideGameOverScreen()
      => SetGameOverScreenVisiblity(false);

    private void SetGameOverScreenVisiblity(bool show)
    {
      CanvasGroup.alpha = show ? 1f : 0f;
      CanvasGroup.interactable = show;
      CanvasGroup.blocksRaycasts = show;
    }

    public void RetryLevel()
      => LoadLevel(Application.loadedLevelName);

    public void LoadLevel(Level level)
      => LoadLevel(level.GetDescription());

    public void QuitGame()
      => Application.Quit();

    private void LoadLevel(string levelName)
    {
      if (!IsLoadingLevel)
        LoadingOperation = Application.LoadLevelAsync(levelName);
    }
  }
}
