using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class LevelHud : View
  {
    [DataBound] public ViewAnimation ShowAnimation = null;
    [DataBound] public ViewAnimation HideAnimation = null;

    public void Show()
      => this.ShowAnimation.StartAnimation();

    public void Hide()
      => this.HideAnimation.StartAnimation();
  }
}