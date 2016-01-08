using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Fart Installer")]
  public class FartInstaller : MonoInstaller
  {
    [SerializeField] private List<Fart.Settings> fartSettings = null;
    
    public override void InstallBindings()
    {
      foreach (var config in this.fartSettings)
      {
        var type = config.Type.GetTypeMapping();

        Container.BindInstance(config).WhenInjectedInto(type);
        Container
          .Bind<FartView>()
          .ToTransientPrefab(config.Prefab)
          .WhenInjectedInto(type);
      }
    }
  }
}