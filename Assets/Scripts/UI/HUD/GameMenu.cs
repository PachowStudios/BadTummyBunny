using UnityEngine;

public class GameMenu : MonoBehaviour
{
	private CanvasGroup canvasGroup;

	public static GameMenu Instance { get; private set; }

	private void Awake()
	{
		Instance = this;

		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void ShowGameOver()
	{
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

	public void HideGameOver()
	{
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	public void LoadLevel(string levelName)
	{
		if (!Application.isLoadingLevel)
		{
			levelName = (levelName == "Retry") ? Application.loadedLevelName : levelName;
			Application.LoadLevel(levelName);
		}
	}

	public void Quit() => Application.Quit();
}
