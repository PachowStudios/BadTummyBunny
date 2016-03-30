using PachowStudios.Assertions;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseHasHealth : IHasHealth
  {
    public abstract int MaxHealth { get; }
    public abstract int Health { get; protected set; }

    public virtual bool IsDead { get; protected set; }

    public abstract void TakeDamage(int damage, Vector2 knockback, Vector2 knockbackDirection);
    public abstract void Kill();

    public virtual void TakeDamage(int damage)
      => TakeDamage(damage, Vector2.zero, Vector2.zero);

    public virtual void Heal(int amountToHeal)
    {
      amountToHeal.Should().BeGreaterThan(0, "because a character can't lose health by healing");

      Health += amountToHeal;
    }
  }
}
