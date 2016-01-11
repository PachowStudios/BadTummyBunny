using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Effect Installer")]
  public class EffectInstaller : MonoInstaller
  {
    [SerializeField] private ExplodeEffectSettings explodeEffetSettings = null;

    public override void InstallBindings()
    {
      Container.BindInstance(this.explodeEffetSettings);
      Container.BindSingle<ExplodeEffect>();
    }
  }
}