using System.Collections.Generic;

namespace PachowStudios.BadTummyBunny
{
  public interface IStatusEffectable : ICharacter
  {
    IEnumerable<IStatusEffect> StatusEffects { get; }

    bool HasStatusEffect(StatusEffectType type);
    void AddStatusEffect(StatusEffectType type);
    void RemoveStatusEffect(StatusEffectType type);
  }
}
