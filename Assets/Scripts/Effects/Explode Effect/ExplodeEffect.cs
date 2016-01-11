using JetBrains.Annotations;
using UnityEngine;
using Zenject;

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
  }
}
