using System;
using PachowStudios.Collections;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StatusEffectFactory : IFactory<StatusEffectType, IStatusEffect>
  {
    [Inject] private DiContainer Container { get; set; }
    [Inject] private IReadOnlyDictionary<StatusEffectType, BaseStatusEffectSettings> StatusEffectSettings { get; set; } 

    public IStatusEffect Create(StatusEffectType type)
    {
      var subContainer = Container.CreateSubContainer();
      var settings = StatusEffectSettings[type];

      subContainer.BindBaseInstance(settings);

      if (settings.Prefab != null)
        subContainer.Bind<IStatusEffectView>().ToTransientPrefab(settings.Prefab);

      return (IStatusEffect)subContainer.Instantiate(type.GetTypeMapping());
    }
  }
}
