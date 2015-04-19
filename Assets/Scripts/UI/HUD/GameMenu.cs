﻿using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour
{
	#region Fields
	private static GameMenu instance;

	private CanvasGroup canvasGroup;
	#endregion

	#region Public Properties
	public static GameMenu Instance
	{ get { return instance; } }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		instance = this;

		canvasGroup = GetComponent<CanvasGroup>();
	}
	#endregion

	#region Public Methods
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

	public void Quit()
	{
		Application.Quit();
	}
	#endregion
}
