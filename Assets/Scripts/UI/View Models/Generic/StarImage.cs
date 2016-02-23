using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class StarImage : View
  {
    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public bool IsCollected = false;

    [DataBound] public ViewSwitcher StarSelector = null;
    [DataBound] public Image EmptyStar = null;
    [DataBound] public Image FilledStar = null;

    private Image CurrentImage => this.IsCollected ? this.FilledStar : this.EmptyStar;

    public override void UpdateLayout()
    {
      base.UpdateLayout();

      this.StarSelector.SwitchTo(CurrentImage);
    }
  }
}
