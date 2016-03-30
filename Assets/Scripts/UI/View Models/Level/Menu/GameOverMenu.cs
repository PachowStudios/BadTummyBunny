using MarkUX;
using MarkUX.Views;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class GameOverMenu : View
  {
    [DataBound] public ViewAnimation ShowAnimation = null;
    [DataBound] public ViewAnimation HideAnimation = null;

    [Inject] private ISceneLoader SceneLoader { get; set; }

    [DataBound]
    public void OnRetryClick()
      => SceneLoader.ReloadScene();

    [DataBound]
    public void OnQuitClick()
      => SceneLoader.QuitGame();

    public void Show()
      => this.ShowAnimation.StartAnimation();

    public void Hide()
      => this.HideAnimation.StartAnimation();
  }
}