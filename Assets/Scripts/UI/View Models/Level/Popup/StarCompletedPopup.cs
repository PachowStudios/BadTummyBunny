using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class StarCompletedPopup : View
  {
    [DataBound] public float PopupDuration = 3f;

    [DataBound] public ViewAnimation ShowHideOffsetAnimation = null;
    [DataBound] public ViewAnimation ShowAlphaAnimation = null;
    [DataBound] public ViewAnimation HideAlphaAnimation = null;

    public void Popup()
    {
      Show();
      Wait.ForSeconds(this.PopupDuration, Hide);
    }

    private void Show()
    {
      this.ShowHideOffsetAnimation.StartAnimation();
      this.ShowAlphaAnimation.StartAnimation();
    }

    private void Hide()
    {
      this.ShowHideOffsetAnimation.ReverseAnimation();
      this.HideAlphaAnimation.StartAnimation();
    }
  }
}