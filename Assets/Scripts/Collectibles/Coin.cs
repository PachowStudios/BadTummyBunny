using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Collectibles/Coin")]
  public class Coin : BaseView
  {
    [UsedImplicitly]
    public enum CoinValue
    {
      Copper = 1,
      Silver = 10,
      Gold = 50
    }

    [SerializeField] private CoinValue value = CoinValue.Copper;

    [Inject] private ExplodeEffect ExplodeEffect { get; set; }

    public int Value => (int)this.value;

    public void Collect()
    {
      PlayCollectSound();
      ExplodeEffect.Explode(Transform, Vector3.zero, SpriteRenderer.sprite);
      Dispose();
    }

    private static void PlayCollectSound() 
      => SoundManager.PlaySFXFromGroup(SfxGroup.Coins);
  }
}
