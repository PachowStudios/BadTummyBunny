using MarkLight;
using MarkLight.Views.UI;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class LevelHud : UIView
  {
    private const string ShownState = "Shown";
    private const string HiddenState = "Hidden";

    public void Show()
      => SetState(ShownState);

    public void Hide()
      => SetState(HiddenState);
  }
}