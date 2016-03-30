using System.Collections.Generic;
using System.Linq;
using PachowStudios.Collections;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Status Effect Installer")]
  public class StatusEffectInstaller : MonoInstaller
  {
    [SerializeField] private List<BaseStatusEffectSettings> statusEffectSettings = null;

    private IReadOnlyDictionary<StatusEffectType, BaseStatusEffectSettings> MappedSettings { get; set; } 

    public override void InstallBindings()
    {
      MappedSettings = this.statusEffectSettings.ToDictionary(s => s.Type).AsReadOnly();

      Container.BindInstance(MappedSettings);
      Container.BindIFactory<StatusEffectType, IStatusEffect>().ToCustomFactory<StatusEffectFactory>();
    }
  }
}