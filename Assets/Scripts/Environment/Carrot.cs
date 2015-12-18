using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class Carrot : MonoBehaviour
  {
    private SpriteRenderer spriteRenderer;

    private SpriteRenderer SpriteRenderer => this.GetComponentIfNull(ref this.spriteRenderer);

    public void Collect()
    {
      PlayCollectSound();
      ExplodeEffect.Instance.Explode(transform, Vector3.zero, SpriteRenderer.sprite);
      Destroy(gameObject);
    }

    private static void PlayCollectSound()
      => SoundManager.PlaySFXFromGroup(SfxGroup.Carrots);
  }
}
