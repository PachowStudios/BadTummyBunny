using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/UI/HUD/Coins")]
  public class CoinsHud : MonoBehaviour,
    IHandles<PlayerCoinsChangedMessage>
  {
    [SerializeField] private Text coinsText = null;
    [SerializeField] private int coinsDigits = 3;

    [Inject(Tags.Player)]
    private IScoreKeeper PlayerScore { get; set; }

    [Inject]
    private IEventAggregator EventAggregator { get; set; }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    private void Awake()
      => this.coinsText.text = new string('0', this.coinsDigits);

    public void Handle(PlayerCoinsChangedMessage message)
      => this.coinsText.text = message.Coins.ToString().PadLeft(this.coinsDigits, '0');
  }
}
