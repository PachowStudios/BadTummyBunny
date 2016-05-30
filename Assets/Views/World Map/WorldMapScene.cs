using MarkLight;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  public class WorldMapScene : View,
    IHandles<LevelSelectedMessage>,
    IHandles<LevelDeselectedMessage>
  {
    [DataBound] public WorldMapLevelPopup LevelPopup;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(LevelSelectedMessage message)
      => this.LevelPopup.Show(message.Level);

    public void Handle(LevelDeselectedMessage message)
      => this.LevelPopup.Hide();
  }
}
