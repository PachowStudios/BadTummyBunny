using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Status Effect Installer")]
  public class StatusEffectInstaller : MonoInstaller
  {
    [SerializeField] private List<BaseStatusEffectSettings> statusEffectSettings = null;

    public override void InstallBindings()
    {
      Container.BindIFactory<StatusEffectType, IStatusEffect>().ToCustomFactory<StatusEffectFactory>();

      foreach (var config in this.statusEffectSettings)
      {
        var type = config.Type.GetTypeMapping();

        Container.BindAbstractInstance(config).WhenInjectedInto(type);

        if (config.Prefab != null)
          Container
            .Bind<IStatusEffectView>()
            .ToTransientPrefab(config.Prefab)
            .WhenInjectedInto(type);
      }
    }
  }
}