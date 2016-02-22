using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class PixelButton : Button
  {
    private const float BaseVerticalTextOffset = 15f;

    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public ElementSize VerticalTextOffset = new ElementSize();

    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public Margin TextOffset = Margin.FromBottom(
      new ElementSize(BaseVerticalTextOffset, ElementSizeUnit.Elements));

    public override void UpdateLayout()
    {
      this.TextOffset.Bottom = new ElementSize(
        this.VerticalTextOffset.Elements + BaseVerticalTextOffset,
        ElementSizeUnit.Elements);

      base.UpdateLayout();
    }

    [DataBound]
    public void OnMouseEnter()
      => ButtonMouseEnter();

    [DataBound]
    public void OnMouseExit()
      => ButtonMouseExit();

    [DataBound]
    public void OnMouseDown()
      => ButtonMouseDown();

    [DataBound]
    public void OnMouseUp()
      => ButtonMouseUp();
  }
}