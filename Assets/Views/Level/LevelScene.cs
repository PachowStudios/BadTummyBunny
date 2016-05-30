using MarkLight;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  public class LevelScene : View,
    IHandles<StarCompletedMessage>,
    IHandles<PlayerDiedMessage>
  {
    [DataBound] public LevelHud LevelHud;
    [DataBound] public GameOverMenu GameOverMenu;
    [DataBound] public StarCompletedPopup StarCompletedPopup;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(StarCompletedMessage message)
      => this.StarCompletedPopup.Popup();

    public void Handle(PlayerDiedMessage message)
    {
      this.LevelHud.Hide();
      this.GameOverMenu.Show();
    }
  }
}