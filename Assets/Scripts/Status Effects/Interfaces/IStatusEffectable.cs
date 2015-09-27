using System.Collections.ObjectModel;

public interface IStatusEffectable
{
  ReadOnlyCollection<IStatusEffect> StatusEffects { get; }

  void AddStatusEffect(IStatusEffect statusEffect);
  void RemoveStatusEffect(IStatusEffect statusEffect);
}
