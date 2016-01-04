using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class WorldMapLevelStar : View
  {
    [DataBound] public ViewSwitcher StarSelector = null;
    [DataBound] public Image EmptyStar = null;
    [DataBound] public Image FilledStar = null;

    [ChangeHandler(nameof(UpdateLayout))]
    public bool IsCollected = false;

    public override void UpdateLayout()
    {
      base.UpdateLayout();

      this.StarSelector.SwitchTo(this.IsCollected ? this.FilledStar : this.EmptyStar);
    }
  }
}
