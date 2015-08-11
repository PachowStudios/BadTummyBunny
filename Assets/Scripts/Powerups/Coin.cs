using UnityEngine;

public class Coin : MonoBehaviour
{
	#region Enums
	public enum CoinValue
	{
		Copper = 1,
		Silver = 10,
		Gold   = 50
	};
	#endregion

	#region Inspector Fields
	[SerializeField]
	private CoinValue value = CoinValue.Copper;
	#endregion

	#region Internal Fields
	private SpriteRenderer spriteRenderer;
	#endregion

	#region Public Properties
	public int Value
	{ get { return (int)value; } }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	#endregion

	#region Public Methods
	public void Collect()
	{
		PlayCollectSound();
		ExplodeEffect.Instance.Explode(transform, Vector3.zero, spriteRenderer.sprite);
		Destroy(gameObject);
	}
	#endregion

	#region Internal Helper Methods
	private void PlayCollectSound()
	{
		SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.Coins));
	}
	#endregion
}
