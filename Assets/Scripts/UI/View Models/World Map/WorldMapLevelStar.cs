using JetBrains.Annotations;
using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class WorldMapLevelStar : View
  {
    [UsedImplicitly] public ViewSwitcher StarSelector = null;
    [UsedImplicitly] public Image EmptyStar = null;
    [UsedImplicitly] public Image FilledStar = null;

    [ChangeHandler(nameof(UpdateLayout))]
    public bool IsCollected = false;

    public override void UpdateLayout()
    {
      base.UpdateLayout();

      this.StarSelector.SwitchTo(
          this.IsCollected
            ? this.FilledStar
            : this.EmptyStar);
    }
  }
}