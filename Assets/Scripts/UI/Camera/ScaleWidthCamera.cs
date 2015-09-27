using DG.Tweening;
using UnityEngine;

[AddComponentMenu("UI/Camera/Scale Width Camera")]
[ExecuteInEditMode]
public sealed class ScaleWidthCamera : MonoBehaviour
{
  public int defaultFOV = 500;
  public bool useWorldSpaceUI = false;
  public RectTransform worldSpaceUI = null;

  public int currentFOV;

  public static ScaleWidthCamera Instance { get; private set; }

  private void OnEnable()
  {
    Instance = this;

    this.currentFOV = this.defaultFOV;
  }

  private void OnPreRender()
  {
    camera.orthographicSize = this.currentFOV / 32f / camera.aspect;

    if (this.useWorldSpaceUI && this.worldSpaceUI != null)
      this.worldSpaceUI.sizeDelta = new Vector2(this.currentFOV / 16f, this.currentFOV / 16f / camera.aspect);
  }

  public void AnimateFOV(int newFOV, float time)
  {
    DOTween.To(() => this.currentFOV, x => this.currentFOV = x, newFOV, time)
           .SetEase(Ease.OutQuint);
  }
}
