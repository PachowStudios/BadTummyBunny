using JetBrains.Annotations;
using MarkUX;

namespace BadTummyBunny.UI.ViewModels
{
  public class WorldMapScreen : View, IHandles<LevelSelectedMessage>, IHandles<LevelDeselectedMessage>
  {
    [UsedImplicitly] public WorldMapLevelPopup LevelPopup = null;

    private void Awake()
      => EventAggregator.Instance.Subscribe(this);

    public void Handle(LevelSelectedMessage message)
    {
      this.LevelPopup.SetLevel(message.Level);
      this.LevelPopup.Show.StartAnimation();
    }

    public void Handle(LevelDeselectedMessage message)
    {
      this.LevelPopup.Hide.StartAnimation();
    }
  }
}
