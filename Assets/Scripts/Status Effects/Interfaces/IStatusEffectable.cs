﻿using System.Collections.ObjectModel;

public interface IStatusEffectable
{
	ReadOnlyCollection<IStatusEffect> StatusEffects { get; }
	ICharacter Character { get; }

	void AddStatusEffect(IStatusEffect statusEffect);
	void RemoveStatusEffect(IStatusEffect statusEffect);
}