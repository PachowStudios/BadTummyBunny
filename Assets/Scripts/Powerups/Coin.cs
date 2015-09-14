using UnityEngine;

public class Coin : MonoBehaviour
{
	public enum CoinValue
	{
		Copper = 1,
		Silver = 10,
		Gold   = 50
	};

	[SerializeField]
	protected CoinValue value = CoinValue.Copper;

	private SpriteRenderer spriteRenderer = null;

	public virtual int Value => (int)value;

	protected SpriteRenderer SpriteRenderer => this.GetComponentIfNull(ref spriteRenderer);

	public virtual void Collect()
	{
		PlayCollectSound();
		ExplodeEffect.Instance.Explode(transform, Vector3.zero, SpriteRenderer.sprite);
		Destroy(gameObject);
	}

	protected virtual void PlayCollectSound() => SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.Coins));
}
