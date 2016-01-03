using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public interface IStatusEffect : ITickable, IDisposable
  {
    string Name { get; }
    StatusEffectType Type { get; }
    IStatusEffectable AffectedCharacter { get; }

    void Attach(IStatusEffectable affectedCharacter);
  }
}
