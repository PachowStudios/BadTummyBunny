using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Effects/Explode Effect View")]
  public sealed class ExplodeEffectView : MonoBehaviour
  {
    private Transform transformComponent;
    private ParticleSystem particleSystemComponent;
    private Renderer particleRendererComponent;

    private Transform Transform => this.GetComponentIfNull(ref this.transformComponent);
    private ParticleSystem ParticleSystem => this.GetComponentIfNull(ref this.particleSystemComponent);
    private Renderer ParticleRenderer => ParticleSystem.GetComponentIfNull(ref this.particleRendererComponent);

    [Inject] private ExplodeEffectSettings Config { get; set; }

    public void Explode([NotNull] Transform target, Vector3 velocity, [NotNull] Sprite sprite, Material material = null)
    {
      Transform.AlignWith(target);

      var spriteWidth = (int)(sprite.bounds.size.x * sprite.pixelsPerUnit);
      var spriteHeight = (int)(sprite.bounds.size.y * sprite.pixelsPerUnit);
      var particles = new List<ParticleSystem.Particle>(spriteWidth * spriteHeight);
      var particle = new ParticleSystem.Particle()
      {
        startSize = 1f / sprite.pixelsPerUnit,
        lifetime = Config.ParticleLifetime,
        startLifetime = Config.ParticleLifetime
      };
      var positionOffset = new Vector2(
        sprite.bounds.extents.x - sprite.bounds.center.x - 0.05f,
        sprite.bounds.extents.y - sprite.bounds.center.y - 0.05f);

      velocity = velocity.Vary(0.5f);
      
      if (material != null)
        ParticleRenderer.material = material;

      for (var widthIndex = 0; widthIndex < spriteWidth; widthIndex++)
        for (var heightIndex = 0; heightIndex < spriteHeight; heightIndex++)
        {
          var color = sprite.texture.GetPixel(
            (int)sprite.rect.x + widthIndex,
            (int)sprite.rect.y + heightIndex);

          // Ignore the pixel if it's transparent
          if (color.a.Abs() <= 0.01f)
            continue;

          particle.position = Transform.TransformPoint(
            (widthIndex / sprite.pixelsPerUnit) - positionOffset.x,
            (heightIndex / sprite.pixelsPerUnit) - positionOffset.y);
          particle.startColor = color;
          particle.velocity = velocity.Vary(3f);

          particles.Add(particle);
        }

      ParticleSystem.SetParticles(particles.ToArray(), particles.Count);
      this.Destroy(Config.Duration);
    }
  }
}