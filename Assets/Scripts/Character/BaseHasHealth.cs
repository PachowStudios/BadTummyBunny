using System;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseHasHealth : MonoBehaviour, IHasHealth
{
  public event Action<int> HealthChanged;

  protected int health = 0;

  public abstract int MaxHealth { get; }
  public abstract int Health { get; protected set; }

  public virtual bool IsDead { get; protected set; }

  public virtual void Heal(int amountToHeal)
  {
    Assert.IsTrue(amountToHeal > 0);

    Health += amountToHeal;
  }

  public virtual void Damage(int damage) => Damage(damage, Vector2.zero, Vector2.zero);

  public abstract void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection);
  public abstract void Kill();

  protected void RaiseHealthChanged(int newHealth) => HealthChanged?.Invoke(newHealth);
}
