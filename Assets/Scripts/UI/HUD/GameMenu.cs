using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class GameMenu : MonoBehaviour
  {
    private CanvasGroup canvasGroup;
    private AsyncOperation loadingOperation;

    public static GameMenu Instance { get; private set; }

    public bool IsLoadingLevel => !(this.loadingOperation?.isDone ?? true);

    private CanvasGroup Group => this.GetComponentIfNull(ref this.canvasGroup);

    private void Awake()
    {
      Instance = this;

      this.canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowGameOver()
    {
      Group.alpha = 1f;
      Group.interactable = true;
      Group.blocksRaycasts = true;
    }

    public void HideGameOver()
    {
      Group.alpha = 0f;
      Group.interactable = false;
      Group.blocksRaycasts = false;
    }

    public void LoadLevel(string levelName)
    {
      if (IsLoadingLevel)
        return;

      if (levelName == "Retry")
        levelName = Application.loadedLevelName;

      this.loadingOperation = Application.LoadLevelAsync(levelName);
    }

    public void Quit()
      => Application.Quit();
  }
}
