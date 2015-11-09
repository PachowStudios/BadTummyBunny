using UnityEngine;

public class GameMenu : MonoBehaviour
{
  private CanvasGroup canvasGroup;

  public static GameMenu Instance { get; private set; }

  public CanvasGroup Group => this.GetComponentIfNull(ref this.canvasGroup);

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
    if (Application.isLoadingLevel)
      return;

    if (levelName == "Retry")
      levelName = Application.loadedLevelName;

    Application.LoadLevel(levelName);
  }

  public void Quit()
    => Application.Quit();
}
