using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Effect Installer")]
  public class EffectInstaller : MonoInstaller
  {
    [SerializeField] private ExplodeEffect.Settings explodeEffetSettings = null;

    public override void InstallBindings()
    {
      Container.BindSingle<ExplodeEffect>();
      Container.BindInstance(this.explodeEffetSettings);
    }
  }
}