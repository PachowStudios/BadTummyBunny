using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class Coin : MonoBehaviour
  {
    public enum CoinValue
    {
      Copper = 1,
      Silver = 10,
      Gold = 50
    };

    [SerializeField] private CoinValue value = CoinValue.Copper;

    private SpriteRenderer spriteRenderer;

    public int Value => (int)this.value;

    private SpriteRenderer SpriteRenderer => this.GetComponentIfNull(ref this.spriteRenderer);

    public void Collect()
    {
      PlayCollectSound();
      ExplodeEffect.Instance.Explode(transform, Vector3.zero, SpriteRenderer.sprite);
      Destroy(gameObject);
    }

    private static void PlayCollectSound() 
      => SoundManager.PlaySFXFromGroup(SfxGroup.Coins);
  }
}
