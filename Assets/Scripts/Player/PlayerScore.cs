using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Player/Player Score")]
public class PlayerScore : MonoBehaviour
{
	#region Fields
	public List<AudioClip> coinSounds;

	private AudioSource audioSource;
	#endregion

	#region Public Properties
	public static PlayerScore Instance { get; private set; }

	public int Coins { get; private set; }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		//audioSource = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == Tags.Coin) CollectCoin(other.GetComponent<Coin>());
	}
	#endregion

	#region Internal Methods
	private void CollectCoin(Coin coin)
	{
		if (coin == null) return;

		coin.Collect();
		Coins++;
		//audioSource.PlayOneShot(coinSounds.GetRandom());
	}
	#endregion
}
