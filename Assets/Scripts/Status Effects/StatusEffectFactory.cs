using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StatusEffectFactory : IFactory<StatusEffectType, IStatusEffect>
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      public List<BaseStatusEffect.BaseSettings> StatusEffectSettings;
    }

    [Inject] private DiContainer Container { get; set; }
    [Inject] private Settings Config { get; set; }

    private Dictionary<StatusEffectType, BaseStatusEffect.BaseSettings> StatusEffectSettings { get; set; }

    [PostInject]
    private void Initialize()
      => StatusEffectSettings = Config.StatusEffectSettings.ToDictionary(s => s.Type);
    
    public IStatusEffect Create(StatusEffectType type)
    {
      var subContainer = Container.CreateSubContainer();
      var settings = StatusEffectSettings[type];
      var mappedType = type.GetTypeMapping();

      subContainer.BindInstance(settings);
      subContainer.BindSingleWithInterfaces(mappedType);

      if (settings.Prefab != null)
        subContainer.Bind<IStatusEffectView>().ToSinglePrefab(settings.Prefab);

      return subContainer.Resolve<IStatusEffect>();
    }
  }
}
