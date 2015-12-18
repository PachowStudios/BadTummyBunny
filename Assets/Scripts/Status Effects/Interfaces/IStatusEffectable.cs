using System.Collections.ObjectModel;

namespace PachowStudios.BadTummyBunny
{
  public interface IStatusEffectable
  {
    ReadOnlyCollection<IStatusEffect> StatusEffects { get; }

    void AddStatusEffect(IStatusEffect statusEffect);
    void RemoveStatusEffect(IStatusEffect statusEffect);
  }
}
