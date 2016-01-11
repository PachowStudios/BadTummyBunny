using System;
using System.Collections.Generic;
using System.Linq.Extensions;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Fart Installer")]
  public class FartInstaller : MonoInstaller
  {
    [SerializeField] private List<FartSettings> fartSettings = null;
    
    public override void InstallBindings()
    {
      Container.BindIFactory<FartType, IFart>().ToCustomFactory<FartFactory>();

      foreach (var config in this.fartSettings)
      {
        var type = config.Type.GetTypeMapping();

        Container.BindAbstractInstance(config).ForEach(c => c.WhenInjectedInto(type));
        Container
          .Bind<FartView>()
          .ToTransientPrefab(config.Prefab)
          .WhenInjectedInto(type);
      }
    }
  }
}