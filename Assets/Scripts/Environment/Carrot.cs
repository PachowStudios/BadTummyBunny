using UnityEngine;

public class Carrot : MonoBehaviour
{
	private SpriteRenderer spriteRenderer = null;

	protected SpriteRenderer SpriteRenderer => this.GetComponentIfNull(ref spriteRenderer);

	public virtual void Collect()
	{
		PlayCollectSound();
		ExplodeEffect.Instance.Explode(transform, Vector3.zero, SpriteRenderer.sprite);
		Destroy(gameObject);
	}

	protected virtual void PlayCollectSound() => SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.Carrots));
}