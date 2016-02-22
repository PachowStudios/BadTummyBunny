using MarkUX;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  public class LevelScreen : View,
    IHandles<PlayerDiedMessage>
  {
    [DataBound] public LevelHud LevelHud = null;
    [DataBound] public GameOverMenu GameOverMenu = null;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(PlayerDiedMessage message)
    {
      this.LevelHud.Hide();
      this.GameOverMenu.Show();
    }
  }
}