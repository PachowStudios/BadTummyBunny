using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class GameMenu : MonoBehaviour, IGameMenu
  {
    private CanvasGroup canvasGroupComponent;

    private CanvasGroup CanvasGroup => this.GetComponentIfNull(ref this.canvasGroupComponent);

    public bool ShowGameOverScreen
    {
      get { return CanvasGroup.IsVisible(); }
      set { CanvasGroup.SetVisibility(value); }
    }
  }
}
