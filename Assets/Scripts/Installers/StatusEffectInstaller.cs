using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Status Effect Installer")]
  public class StatusEffectInstaller : MonoInstaller
  {
    [SerializeField] private List<BaseStatusEffect.BaseSettings> statusEffectSettings = null;

    public override void InstallBindings()
    {
      foreach (var config in this.statusEffectSettings)
      {
        var type = config.Type.GetTypeMapping();

        Container.BindInstance(config).WhenInjectedInto(type);

        if (config.Prefab != null)
          Container
            .Bind<IStatusEffectView>()
            .ToTransientPrefab(config.Prefab)
            .WhenInjectedInto(type);
      }
    }
  }
}