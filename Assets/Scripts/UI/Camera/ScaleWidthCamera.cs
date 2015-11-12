using DG.Tweening;
using UnityEngine;

[AddComponentMenu("UI/Camera/Scale Width Camera")]
[ExecuteInEditMode]
public sealed class ScaleWidthCamera : MonoBehaviour
{
  [SerializeField] public int defaultFOV = 500;
  [SerializeField] public bool useWorldSpaceUI;
  [SerializeField] public RectTransform worldSpaceUI;

  public int CurrentFOV { get; set; }

  private Camera controlledCamera;

  public static ScaleWidthCamera Instance { get; private set; }

  private Camera Camera => this.GetComponentIfNull(ref this.controlledCamera);

  private void OnEnable()
  {
    Instance = this;

    CurrentFOV = this.defaultFOV;
  }

  private void OnPreRender()
  {
    Camera.orthographicSize = CurrentFOV / 32f / Camera.aspect;

    if (this.useWorldSpaceUI && this.worldSpaceUI != null)
      this.worldSpaceUI.sizeDelta =
        new Vector2(
          CurrentFOV / 16f,
          CurrentFOV / 16f / Camera.aspect);
  }

  public void AnimateFOV(int newFOV, float time)
  {
    DOTween.To(() => CurrentFOV, x => CurrentFOV = x, newFOV, time)
      .SetEase(Ease.OutQuint);
  }
}
