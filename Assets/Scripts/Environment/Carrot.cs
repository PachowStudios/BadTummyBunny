using UnityEngine;

public class Carrot : MonoBehaviour
{
	#region Fields
	private SpriteRenderer spriteRenderer;
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
		SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.Carrots));
	}
	#endregion
}
