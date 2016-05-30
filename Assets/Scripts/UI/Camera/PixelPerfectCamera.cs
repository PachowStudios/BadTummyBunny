using System.Diagnostics;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [ExecuteInEditMode, AddComponentMenu("Bad Tummy Bunny/Camera/Pixel Perfect Camera")]
  public sealed class PixelPerfectCamera : MonoBehaviour
  {
    [SerializeField] private int referenceWidth = 256;
    [SerializeField] private int pixelsPerUnit = 16;
    [SerializeField] private GameObject uiRoot = null;

    private Camera cameraComponent;
    private RectTransform uiTransformComponent;

    private Camera Camera => this.GetComponentIfNull(ref this.cameraComponent);
    private RectTransform UITransform => this.uiRoot.GetComponentInChildrenIfNull(ref this.uiTransformComponent);

    [Conditional("UNITY_EDITOR")]
    private void Update() { }

    private void OnPreRender()
    {
      var referenceOrthographicSize = CalculateOrthographicSize(this.referenceWidth);
      var orthographicSize = CalculateOrthographicSize(Screen.width);
      var multiplier = Mathf.Max(1, Mathf.Round(orthographicSize / referenceOrthographicSize));

      Camera.orthographicSize = orthographicSize / multiplier;

      if (UITransform != null)
        UpdateUISize(multiplier);
    }

    private void UpdateUISize(float multiplier)
    {
      var uiWidth = (float)Screen.width / this.pixelsPerUnit / multiplier;

      UITransform.sizeDelta = new Vector2(uiWidth, uiWidth / Camera.aspect);
    }

    private float CalculateOrthographicSize(int width)
      => width / (this.pixelsPerUnit * 2f) / Camera.aspect;
  }
}
