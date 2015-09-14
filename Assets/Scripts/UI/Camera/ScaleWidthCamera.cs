using DG.Tweening;
using UnityEngine;

[AddComponentMenu("UI/Camera/Scale Width Camera")]
[ExecuteInEditMode]
public sealed class ScaleWidthCamera : MonoBehaviour
{
	public int defaultFOV = 500;
	public bool useWorldSpaceUI = false;
	public RectTransform worldSpaceUI;

	public int FOV;

	public static ScaleWidthCamera Instance { get; private set; }

	private void OnEnable()
	{
		Instance = this;

		FOV = defaultFOV;
	}

	private void OnPreRender()
	{
		camera.orthographicSize = FOV / 32f / camera.aspect;

		if (useWorldSpaceUI && worldSpaceUI != null)
			worldSpaceUI.sizeDelta = new Vector2(FOV / 16f, FOV / 16f / camera.aspect);
	}

	public void AnimateFOV(int newFOV, float time)
	{
		DOTween.To(() => FOV, x => FOV = x, newFOV, time)
			.SetEase(Ease.OutQuint);
	}
}
