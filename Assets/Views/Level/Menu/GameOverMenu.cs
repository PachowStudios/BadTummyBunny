using MarkLight;
using MarkLight.Views.UI;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class GameOverMenu : UIView
  {
    private const string ShownState = "Shown";
    private const string HiddenState = "Hidden";

    [Inject] private ISceneLoader SceneLoader { get; set; }

    [DataBound]
    public void OnRetryClicked()
      => SceneLoader.ReloadScene();

    [DataBound]
    public void OnQuitClicked()
      => SceneLoader.QuitGame();

    public void Show()
      => SetState(ShownState);

    public void Hide()
      => SetState(HiddenState);

    public override void Initialize()
    {
      base.Initialize();

      Hide();
    }
  }
}