using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class PixelButton : Button
  {
    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public Margin TextOffset = new Margin();
  }
}