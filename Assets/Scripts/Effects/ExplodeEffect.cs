﻿using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using System.Collections;
using System.Collections.Generic;
using Particle = UnityEngine.ParticleSystem.Particle;

namespace PachowStudios.BadTummyBunny
{
  public class ExplodeEffect
  {
    [Inject] private ExplodeEffectSettings Config { get; set; }
    [Inject] private IInstantiator Instantiator { get; set; }

    public void Explode([NotNull] Transform target, Vector3 velocity, [NotNull] Sprite sprite, Material material = null)
      => Instantiator
        .InstantiatePrefab(Config.ExplosionPrefab)
        .Explode(target, velocity, sprite, material);

    public sealed class SpriteExplosion : MonoBehaviour
    {
      [SerializeField] private float duration = 5f;
      [SerializeField] private float particleLifetime = 1f;
      [SerializeField] private string sortingLayer = "Foreground";
      [SerializeField] private int sortingOrder = 1;

      private Transform transformComponent;
      private ParticleSystem particleSystemComponent;
      private Renderer particleRendererComponent;

      private Transform Transform => this.GetComponentIfNull(ref this.transformComponent);
      private ParticleSystem ParticleSystem => this.GetComponentIfNull(ref this.particleSystemComponent);
      private Renderer ParticleRenderer => ParticleSystem.GetComponentIfNull(ref this.particleRendererComponent);

      public void Explode([NotNull] Transform target, Vector3 velocity, [NotNull] Sprite sprite, Material material = null)
      {
        Transform.AlignWith(target);
        StartCoroutine(ExplodeCoroutine(velocity, sprite, material));
      }

      private IEnumerator ExplodeCoroutine(Vector3 velocity, [NotNull] Sprite sprite, Material material = null)
      {
        var spriteWidth = (int)(sprite.bounds.size.x * sprite.pixelsPerUnit);
        var spriteHeight = (int)(sprite.bounds.size.y * sprite.pixelsPerUnit);
        var particles = new List<Particle>(spriteWidth * spriteHeight);
        var particle = new Particle() { startSize = 1f / sprite.pixelsPerUnit };
        var positionOffset = new Vector2(
          sprite.bounds.extents.x - sprite.bounds.center.x - 0.05f,
          sprite.bounds.extents.y - sprite.bounds.center.y - 0.05f);

        velocity = velocity.Vary(0.5f);

        ParticleRenderer.sortingLayerName = this.sortingLayer;
        ParticleRenderer.sortingOrder = this.sortingOrder;

        if (material != null)
          ParticleRenderer.material = material;

        for (var widthIndex = 0; widthIndex < spriteWidth; widthIndex++)
          for (var heightIndex = 0; heightIndex < spriteHeight; heightIndex++)
          {
            var color = sprite.texture.GetPixel(
              (int)sprite.rect.x + widthIndex,
              (int)sprite.rect.y + heightIndex);

            if (color.a.Abs() <= 0.01f)
              continue;

            particle.position = Transform.TransformPoint(
              (widthIndex / sprite.pixelsPerUnit) - positionOffset.x,
              (heightIndex / sprite.pixelsPerUnit) - positionOffset.y);
            particle.startColor = color;
            particle.startLifetime = particle.lifetime = this.particleLifetime;
            particle.velocity = velocity.Vary(3f);

            particles.Add(particle);
          }

        ParticleSystem.SetParticles(particles.ToArray(), particles.Count);
        this.Destroy(this.duration);

        yield return null;
      }
    }
  }
}
