using MarkUX;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  public class WorldMapScreen : View,
    IHandles<LevelSelectedMessage>,
    IHandles<LevelDeselectedMessage>
  {
    [DataBound] public WorldMapLevelPopup LevelPopup = null;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(LevelSelectedMessage message)
    {
      this.LevelPopup.SetLevel(message.Level);
      this.LevelPopup.ShowAnimation.StartAnimation();
    }

    public void Handle(LevelDeselectedMessage message)
    {
      this.LevelPopup.HideAnimation.StartAnimation();
    }
  }
}
