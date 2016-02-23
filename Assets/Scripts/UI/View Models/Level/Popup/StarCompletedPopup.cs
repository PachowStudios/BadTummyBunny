using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class StarCompletedPopup : View
  {
    [DataBound] public ViewAnimation ShowHideOffsetAnimation = null;
    [DataBound] public ViewAnimation ShowAlphaAnimation = null;
    [DataBound] public ViewAnimation HideAlphaAnimation = null;

    public void Show()
    {
      this.ShowHideOffsetAnimation.StartAnimation();
      this.ShowAlphaAnimation.StartAnimation();
    }

    public void Hide()
    {
      this.ShowHideOffsetAnimation.ReverseAnimation();
      this.HideAlphaAnimation.StartAnimation();
    }
  }
}