using System;

namespace PachowStudios.BadTummyBunny
{
  public interface IStatusEffect
  {
    event Action<IStatusEffect> Deactivated;

    string StatusEffectName { get; }
    ICharacter AffectedCharacter { get; }
    bool IsActive { get; }
    bool IsDisposed { get; }

    void Activate(ICharacter affectedCharacter);
  }
}
