using UnityEngine;

public class ExplodeEffect : MonoBehaviour
{
  [SerializeField] private SpriteExplosion explosionPrefab = null;

  public static ExplodeEffect Instance { get; private set; }

  private void Awake()
    => Instance = this;

  public void Explode(Transform target, Vector3 velocity, Sprite sprite, Material material = null)
  {
    if (!sprite)
      return;

    var explosionInstance = (SpriteExplosion)Instantiate(this.explosionPrefab, target.position, target.rotation);
    explosionInstance.transform.parent = transform;
    explosionInstance.transform.localScale = target.localScale;

    explosionInstance.Explode(velocity, sprite, material);
  }
}
