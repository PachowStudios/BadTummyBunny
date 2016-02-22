using System;
using MarkUX;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  [InternalView]
  public class HudCoinCounter : View,
    IHandles<PlayerCoinsChangedMessage>
  {
    private const int CoinsDigits = 4;

    [DataBound] public string CoinsText = '0'.Repeat(CoinsDigits);

    [Inject] private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Handle(PlayerCoinsChangedMessage message)
      => SetValue(() => this.CoinsText, message.Coins.ToString().PadLeft(CoinsDigits, '0'));
  }
}