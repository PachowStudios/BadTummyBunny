using System;

public interface IStatusEffect
{
	event Action<IStatusEffect> Deactivated;

	string StatusEffectName { get; }
	IStatusEffectable AffectedObject { get; }

	void Activate(IStatusEffectable affectedObject);
	void Deactivate();
	void UpdateEffect();
}