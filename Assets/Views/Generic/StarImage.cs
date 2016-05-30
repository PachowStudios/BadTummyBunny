using MarkLight;
using MarkLight.Views.UI;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class StarImage : UIView
  {
    [DataBound, ChangeHandler(nameof(OnIsCollectedChanged))]
    public _bool IsCollected;

    [DataBound] public ViewSwitcher StarSelector;
    [DataBound] public Image EmptyStar;
    [DataBound] public Image FilledStar;

    private Image CurrentImage => this.IsCollected ? this.FilledStar : this.EmptyStar;

    private void OnIsCollectedChanged()
      => this.StarSelector.SwitchTo(CurrentImage);
  }
}
