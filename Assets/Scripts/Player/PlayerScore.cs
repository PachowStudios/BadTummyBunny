using UnityEngine;

[AddComponentMenu("Player/Player Score")]
public class PlayerScore : MonoBehaviour
{
	#region Public Properties
	public static PlayerScore Instance { get; private set; }

	public int Coins { get; private set; }
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

		coin.Collect();
		Coins++;
	}
	#endregion
}
