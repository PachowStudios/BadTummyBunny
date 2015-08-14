using UnityEngine;

[AddComponentMenu("Player/Player Score")]
public class PlayerScore : MonoBehaviour
{
	#region Events
	public delegate void OnCoinsChangedHandler(int newCoins);

	public event OnCoinsChangedHandler OnCoinsChanged;
	#endregion

	#region Internal Fields
	private int coins;
	#endregion

	#region Public Properties
	public static PlayerScore Instance { get; private set; }

	public int Coins
	{
		get { return coins; }
		set
		{
			coins = Mathf.Max(0, value);

			if (OnCoinsChanged != null) OnCoinsChanged(coins);
		}
	}
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == Tags.Coin) CollectCoin(other.GetComponent<Coin>());
	}
	#endregion

	#region Internal Helper Methods
	private void CollectCoin(Coin coin)
	{
		if (coin == null) return;

		Coins += coin.Value;
		coin.Collect();
	}
	#endregion
}
