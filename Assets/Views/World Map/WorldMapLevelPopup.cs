using System.Linq;
using System.Linq.Extensions;
using MarkLight;
using MarkLight.Views.UI;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class WorldMapLevelPopup : UIView
  {
    private const string ShownState = "Shown";
    private const string HiddenState = "Hidden";

    private WorldMapLevel level;

    [DataBound] public _string LevelName;

    [DataBound] public ObservableList<Field<bool>> Stars { get; } =
      EnumerableHelper.Repeat<Field<bool>>(() => false, 3).ToObservableList();

    [DataBound]
    public void OnClicked()
      => this.level.LoadScene();

    public void Show(WorldMapLevel newLevel)
    {
      UpdateLevel(newLevel);
      SetState(ShownState);
    }

    public void Hide()
      => SetState(HiddenState);

    public override void SetDefaultValues()
    {
      base.SetDefaultValues();

      this.LevelName.DirectValue = "Level X";
    }

    public override void Initialize()
    {
      base.Initialize();

      Hide();
    }

    private void UpdateLevel(WorldMapLevel newLevel)
    {
      this.level = newLevel;
      this.LevelName.Value = newLevel.LevelName;
      Stars.ReplaceAll(
        newLevel.Stars.Select(s => new Field<bool>(s.IsCompleted)));
    }
  }
}