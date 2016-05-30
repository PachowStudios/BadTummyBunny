using MarkLight;
using MarkLight.Views.UI;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class StarCompletedPopup : UIView
  {
    private const string ShownState = "Shown";
    private const string HiddenState = "Hidden";

    [DataBound] public float PopupDuration { get; set; } = 3f;

    public void Popup()
    {
      Show();
      Wait.ForSeconds(PopupDuration, Hide);
    }

    private void Show()
      => SetState(ShownState);

    private void Hide()
      => SetState(HiddenState);

    public override void Initialize()
    {
      base.Initialize();

      Hide();
    }
  }
}