using System.Collections.Generic;
using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class WorldMapLevelPopup : View
  {
    [DataBound] public List StarList = null;

    [DataBound] public ViewAnimation ShowAnimation = null;
    [DataBound] public ViewAnimation HideAnimation = null;

    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public string LevelName = "Level X";

    [DataBound, ChangeHandler(nameof(UpdateLayout))]
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