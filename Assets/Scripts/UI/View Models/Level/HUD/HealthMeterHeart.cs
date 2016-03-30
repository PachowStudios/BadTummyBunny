using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class HealthMeterHeart : View
  {
    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public int Health;

    [DataBound] public ViewSwitcher HeartSelector = null;

    public override void UpdateLayout()
    {
      base.UpdateLayout();

      this.HeartSelector.SwitchTo(this.Health);
    }
  }
}