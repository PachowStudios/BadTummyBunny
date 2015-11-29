using System.Collections.Generic;
using JetBrains.Annotations;
using MarkUX;
using MarkUX.Views;

namespace BadTummyBunny.UI
{
  [InternalView]
  public class WorldMapLevelPopup : View
  {
    [UsedImplicitly] public List StarList = null;

    [UsedImplicitly] public ViewAnimation ShowAnimation = null;
    [UsedImplicitly] public ViewAnimation HideAnimation = null;

    [ChangeHandler(nameof(UpdateLayout))]
    public string LevelName = "Level X";

    [ChangeHandler(nameof(UpdateLayout))]
    public List<Field<bool>> Stars = new List<Field<bool>>();

    public void SetLevel(WorldMapLevel level)
    {
      SetValue(() => this.LevelName, level.LevelName);
      UpdateStars(level.CollectedStars, level.PossibleStars);
    }

    private void UpdateStars(int collected, int possible)
    {
      this.Stars.Clear();

      for (var i = 0; i < possible; i++)
        this.Stars.Add(new Field<bool>(i < collected));

      SetChanged(() => this.Stars);
    }
  }
}