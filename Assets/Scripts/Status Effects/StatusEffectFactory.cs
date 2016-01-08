using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StatusEffectFactory : IFactory<StatusEffectType, IStatusEffect>
  {
    [Inject] private IInstantiator Instantiator { get; set; }

    public IStatusEffect Create(StatusEffectType type)
      => (IStatusEffect)Instantiator.Instantiate(type.GetTypeMapping());
  }
}
