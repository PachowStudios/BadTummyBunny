using System;
using UnityEngine;

public interface IHasHealth
{
	event Action<int> HealthChanged;

	int MaxHealth { get; }
	int Health { get; }
	bool IsDead { get; }

	void Heal(int amountToHeal);
	void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection);
	void Kill();
}