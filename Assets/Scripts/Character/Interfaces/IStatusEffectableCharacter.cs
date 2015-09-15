using System.Collections.ObjectModel;

public interface IStatusEffectableCharacter : ICharacter
{
	ReadOnlyCollection<IStatusEffect> StatusEffects { get; }

	void AddStatusEffect(IStatusEffect statusEffect);
	void RemoveStatusEffect(IStatusEffect statusEffect);
}