using System;

public interface IStatusEffect
{
	event Action<IStatusEffect> Deactivated;

	string StatusEffectName { get; }
	ICharacter AffectedCharacter { get; }

	void Activate(ICharacter affectedCharacter);
	void UpdateEffect();
}