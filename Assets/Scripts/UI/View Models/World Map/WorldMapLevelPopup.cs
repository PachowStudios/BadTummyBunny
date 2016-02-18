using System.Collections.Generic;
using MarkUX;
using MarkUX.Views;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class WorldMapLevelPopup : View
  {
    [DataBound] public ViewAnimation ShowAnimation = null;
    [DataBound] public ViewAnimation HideAnimation = null;

    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public string LevelName = "Level X";

    [DataBound, ChangeHandler(nameof(UpdateLayout))]
    public List<Field<bool>> Stars = new List<Field<bool>>(3);

    private WorldMapLevel level;

    public WorldMapLevel Level
    {
      private get { return this.level; }
      set
      {
        this.level = value;

        SetValue(() => this.LevelName, Level.LevelName);
        UpdateStars();
      }
    }

    [DataBound]
    public void OnClick()
      => Level.LoadScene();

    public void Show()
      => this.ShowAnimation.StartAnimation();

    public void Hide()
      => this.HideAnimation.StartAnimation();

    private void UpdateStars()
    {
      this.Stars.Clear();

      foreach (var star in Level.Stars)
        this.Stars.Add(star.IsCompleted);

      SetChanged(() => this.Stars);
    }
  }
}