using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class SpriteExplosion : MonoBehaviour
{
  [SerializeField] private float lifetime = 1f;
  [SerializeField] private float systemLifetime = 5f;
  [SerializeField] private string sortingLayer = "Foreground";
  [SerializeField] private int sortingOrder = 1;

  public Material Material { get; set; }

  private IEnumerator DoExplode(Vector3 velocity, Sprite sprite)
  {
    var partSystem = GetComponent<ParticleSystem>();
    var particles = new List<ParticleSystem.Particle>();
    var currentParticle = new ParticleSystem.Particle();

    partSystem.renderer.sortingLayerName = this.sortingLayer;
    partSystem.renderer.sortingOrder = this.sortingOrder;
    currentParticle.size = 1f / sprite.pixelsPerUnit;

    var randomTranslate = new Vector2(Random.Range(-0.5f, 0.5f),
                                      Random.Range(-0.5f, 0.5f));

    if (Material != null)
      partSystem.renderer.material = Material;

    for (var i = 0; i < sprite.bounds.size.x * sprite.pixelsPerUnit; i++)
    {
      for (var j = 0; j < sprite.bounds.size.y * sprite.pixelsPerUnit; j++)
      {
        var particleColor = sprite.texture.GetPixel((int)sprite.rect.x + i,
                                                    (int)sprite.rect.y + j);

        if (Math.Abs(particleColor.a) <= 0.01f)
          continue;

        var positionOffset = new Vector2(sprite.bounds.extents.x - sprite.bounds.center.x - 0.05f,
                                         sprite.bounds.extents.y - sprite.bounds.center.y - 0.05f);

        var particlePosition = transform.TransformPoint((i / sprite.pixelsPerUnit) - positionOffset.x,
                                                        (j / sprite.pixelsPerUnit) - positionOffset.y, 0);

        currentParticle.position = particlePosition;
        currentParticle.rotation = 0f;
        currentParticle.color = particleColor;
        currentParticle.startLifetime = currentParticle.lifetime = this.lifetime;
        currentParticle.velocity = new Vector2(velocity.x + Random.Range(-3f, 3f),
                                               velocity.y + Random.Range(-3f, 3f));
        currentParticle.velocity += randomTranslate.ToVector3();

        particles.Add(currentParticle);
      }
    }

    partSystem.SetParticles(particles.ToArray(), particles.Count);
    gameObject.Destroy(this.systemLifetime);

    yield return null;
  }

  public void Explode(Vector3 velocity, Sprite sprite) 
    => StartCoroutine(DoExplode(velocity, sprite));
}
