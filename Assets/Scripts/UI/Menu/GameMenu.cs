using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/UI/Menu/Game Menu")]
  public class GameMenu : MonoBehaviour, IGameMenu
  {
    public bool ShowGameOverScreen
    {
      get { return CanvasGroup.IsVisible(); }
      set { CanvasGroup.SetVisibility(value); }
    }

    [Inject] private ISceneLoader SceneLoader { get; set; }

    private CanvasGroup canvasGroupComponent;
    private CanvasGroup CanvasGroup => this.GetComponentIfNull(ref this.canvasGroupComponent);

    [UsedImplicitly]
    public void Retry()
      => SceneLoader.ReloadScene();

    [UsedImplicitly]
    public void Quit()
      => SceneLoader.QuitGame();
  }
}
