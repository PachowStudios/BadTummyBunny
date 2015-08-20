using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/HUD/Coins")]
public class CoinsHud : MonoBehaviour
{
	#region Inspector Fields
	public Text coinsText;
	public int coinsDigits = 3;
	#endregion

	#region Public Properties
	public static CoinsHud Instance { get; private set; }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		coinsText.text = new string('0', coinsDigits);
	}

	private void Start()
	{
		PlayerScore.Instance.CoinsChanged += OnCoinsChanged;
	}

	private void OnDestroy()
	{
		PlayerScore.Instance.CoinsChanged -= OnCoinsChanged;
	}
	#endregion

	#region Internal Helper Methods
	private void OnCoinsChanged(int newCoins)
	{
		coinsText.text = newCoins.ToString().PadLeft(coinsDigits, '0');
	}
	#endregion
}
