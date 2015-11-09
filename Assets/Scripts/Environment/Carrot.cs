using UnityEngine;

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

  private void PlayCollectSound() 
    => SoundManager.PlaySFXFromGroup(SfxGroups.Carrots);
}
