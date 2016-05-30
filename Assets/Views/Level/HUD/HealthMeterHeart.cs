using MarkLight;
using MarkLight.Views.UI;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class HealthMeterHeart : UIView
  {
    [DataBound, ChangeHandler(nameof(OnHealthChanged))]
    public _int Health;

    [DataBound] public ViewSwitcher HeartSelector;

    private void OnHealthChanged()
      => this.HeartSelector.SwitchTo(this.Health.Value);
  }
}