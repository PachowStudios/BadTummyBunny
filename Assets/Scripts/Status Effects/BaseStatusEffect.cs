using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseStatusEffect : IStatusEffect
  {
    [InstallerSettings]
    public abstract class BaseSettings : ScriptableObject
    {
      public StatusEffectType Type;
      public string Name = "New Status Effect";
      public GameObject Prefab;
    }

    [Inject] private BaseSettings Config { get; set; }

    public IStatusEffectable AffectedCharacter { get; private set; }

    public string Name => Config.Name;
    public StatusEffectType Type => Config.Type;

    public virtual void Attach(IStatusEffectable affectedCharacter)
    {
      AffectedCharacter = affectedCharacter;
    }

    public virtual void Dispose() { }

    public virtual void Tick() { }
  }
}
