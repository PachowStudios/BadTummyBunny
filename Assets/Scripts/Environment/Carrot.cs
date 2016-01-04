using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class Carrot : MonoBehaviour
  {
    private SpriteRenderer spriteRenderer;

    [Inject] private ExplodeEffect ExplodeEffect { get; set; }

    private SpriteRenderer SpriteRenderer => this.GetComponentIfNull(ref this.spriteRenderer);

    public void Collect()
    {
      PlayCollectSound();
      ExplodeEffect.Explode(transform, Vector3.zero, SpriteRenderer.sprite);
      Destroy(gameObject);
    }

    private static void PlayCollectSound()
      => SoundManager.PlaySFXFromGroup(SfxGroup.Carrots);
  }
}
