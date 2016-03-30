using System.Diagnostics;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [ExecuteInEditMode, AddComponentMenu("Bad Tummy Bunny/Camera/Pixel Perfect Camera")]
  public sealed class PixelPerfectCamera : MonoBehaviour
  {
    [SerializeField] private int referenceWidth = 256;
    [SerializeField] private int pixelsPerUnit = 16;
    [SerializeField] private RectTransform worldSpaceUI = null;

    private Camera cameraComponent;

    private Camera Camera => this.GetComponentIfNull(ref this.cameraComponent);

    [Conditional("UNITY_EDITOR")]
    private void Update() { }

    private void OnPreRender()
    {
      var referenceOrthographicSize = CalculateOrthographicSize(this.referenceWidth);
      var orthographicSize = CalculateOrthographicSize(Screen.width);
      var multiplier = Mathf.Max(1, Mathf.Round(orthographicSize / referenceOrthographicSize));

      Camera.orthographicSize = orthographicSize / multiplier;

      if (this.worldSpaceUI != null)
        UpdateUISize(multiplier);
    }

    private void UpdateUISize(float multiplier)
    {
      var uiWidth = (float)Screen.width / this.pixelsPerUnit / multiplier;

      this.worldSpaceUI.sizeDelta = new Vector2(uiWidth, uiWidth / Camera.aspect);
    }

    private float CalculateOrthographicSize(int width)
      => width / (this.pixelsPerUnit * 2f) / Camera.aspect;
  }
}
