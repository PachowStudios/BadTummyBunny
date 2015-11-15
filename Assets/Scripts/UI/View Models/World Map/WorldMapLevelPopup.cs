using JetBrains.Annotations;
using MarkUX;
using MarkUX.Views;

namespace BadTummyBunny.UI.ViewModels
{
  [InternalView]
  public class WorldMapLevelPopup : View
  {
    [UsedImplicitly] public ViewAnimation Show = null;
    [UsedImplicitly] public ViewAnimation Hide = null;

    public string LevelName;

    public void SetLevel(WorldMapLevel level)
    {
      SetValue(() => this.LevelName, level.name);
    }
  }
}