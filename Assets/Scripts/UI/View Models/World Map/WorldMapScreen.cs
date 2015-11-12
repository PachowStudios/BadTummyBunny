using JetBrains.Annotations;
using MarkUX;

namespace BadTummyBunny.UI.ViewModels
{
  public class WorldMapScreen : View
  {
    [UsedImplicitly]
    private WorldMapLevelPopup levelPopup;

    private void Start()
    {
      WorldMap.Instance.LevelSelected += OnLevelSelected;
      WorldMap.Instance.LevelDeselected += OnLevelDeselected;
    }

    private void OnDestroy()
    {
      WorldMap.Instance.LevelSelected -= OnLevelSelected;
      WorldMap.Instance.LevelDeselected -= OnLevelDeselected;
    }

    private void OnLevelSelected(WorldMapLevel level)
    {
      
    }

    private void OnLevelDeselected(WorldMapLevel level)
    {
      
    }
  }
}
