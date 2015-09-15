using System;

public interface IStatusEffect
{
	event Action<IStatusEffect> Deactivated;

	string StatusEffectName { get; }
	IStatusEffectableCharacter AffectedCharacter { get; }

	void Activate(IStatusEffectableCharacter affectedCharacter);
	void UpdateEffect();
}