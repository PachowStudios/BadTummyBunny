using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class FartFactory : IFactory<FartType, IFart>
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      public List<Fart.Settings> FartSettings;
    }

    [Inject] private DiContainer Container { get; set; }
    [Inject] private Settings Config { get; set; }

    private Dictionary<FartType, Fart.Settings> FartSettings { get; set; }

    [PostInject]
    private void Initialize()
      => FartSettings = Config.FartSettings.ToDictionary(s => s.Type);

    public IFart Create(FartType type)
    {
      var subContainer = Container.CreateSubContainer();
      var settings = FartSettings[type];
      var mappedType = type.GetTypeMapping();

      subContainer.Bind(settings.GetType()).ToInstance(settings);
      subContainer.BindAllInterfacesToSingle(mappedType);
      subContainer.Bind(mappedType).ToSingle();
      subContainer.Bind<FartView>().ToSinglePrefab(settings.Prefab);

      return subContainer.Resolve<IFart>();
    }
  }
}