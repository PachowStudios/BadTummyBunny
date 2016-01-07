using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Environment/Carrot")]
  public class Carrot : BaseView
  {
    [Inject] private ExplodeEffect ExplodeEffect { get; set; }

    public void Collect()
    {
      PlayCollectSound();
      ExplodeEffect.Explode(transform, Vector3.zero, SpriteRenderer.sprite);
      Dispose();
    }

    private static void PlayCollectSound()
      => SoundManager.PlaySFXFromGroup(SfxGroup.Carrots);
  }
}
