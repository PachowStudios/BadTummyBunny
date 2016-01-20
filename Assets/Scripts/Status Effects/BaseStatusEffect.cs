using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class BaseStatusEffectSettings : ScriptableObject
  {
    public StatusEffectType Type;
    public string Name = "New Status Effect";
    public GameObject Prefab;
  }

  public abstract class BaseStatusEffect<TConfig> : IStatusEffect
    where TConfig : BaseStatusEffectSettings
  {
    protected abstract TConfig Config { get; set; }

    public IStatusEffectable AffectedCharacter { get; private set; }

    public string Name => Config.Name;
    public StatusEffectType Type => Config.Type;

    public virtual void Attach(IStatusEffectable affectedCharacter)
      => AffectedCharacter = affectedCharacter;

    public virtual void Detach() { }

    public virtual void Tick() { }
  }
}
