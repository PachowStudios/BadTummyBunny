using MarkUX;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  public class LevelScreen : View,
    IHandles<StarCompletedMessage>,
    IHandles<PlayerDiedMessage>
  {
    [DataBound] public LevelHud LevelHud = null;
    [DataBound] public GameOverMenu GameOverMenu = null;
    [DataBound] public StarCompletedPopup StarCompletedPopup = null;

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