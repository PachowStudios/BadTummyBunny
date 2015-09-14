using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/HUD/Coins")]
public class CoinsHud : MonoBehaviour
{
	[SerializeField]
	protected Text coinsText;
	[SerializeField]
	protected int coinsDigits = 3;

	public static CoinsHud Instance { get; private set; }

	protected virtual void Awake()
	{
		Instance = this;

		coinsText.text = new string('0', coinsDigits);
	}

	protected virtual void Start() => Player.Instance.Score.CoinsChanged += OnCoinsChanged;

	protected virtual void OnDestroy() => Player.Instance.Score.CoinsChanged -= OnCoinsChanged;

	protected virtual void OnCoinsChanged(int newCoins) => coinsText.text = newCoins.ToString().PadLeft(coinsDigits, '0');
}
