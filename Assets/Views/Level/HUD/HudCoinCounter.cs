using MarkLight;
using MarkLight.Views.UI;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class HudCoinCounter : UIView,
    IHandles<PlayerCoinsChangedMessage>
  {
    private const int CoinsDigits = 4;

    [DataBound] public _string CoinsText;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(PlayerCoinsChangedMessage message)
      => this.CoinsText.Value = message.Coins.ToString().PadLeft(CoinsDigits, '0');

    public override void SetDefaultValues()
    {
      base.SetDefaultValues();

      this.CoinsText.DirectValue = new string('0', CoinsDigits);
    }
  }
}