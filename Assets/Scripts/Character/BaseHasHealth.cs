using System;
using UnityEngine;

public abstract class BaseHasHealth : MonoBehaviour, IHasHealth
{
	public event Action<int> HealthChanged;

	protected int health = 0;

	public abstract int MaxHealth { get; }
	public abstract int Health { get; protected set; }

	public virtual bool IsDead { get; protected set; }

	public virtual void Heal(int amountToHeal)
	{
		if (amountToHeal <= 0) Debug.LogError($"{nameof(amountToHeal)} cannot be 0 or less. Was {amountToHeal}");

		Health += amountToHeal;
	}

	public abstract void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection);
	public abstract void Kill();

	protected void RaiseHealthChanged(int newHealth) => HealthChanged?.Invoke(newHealth);
}