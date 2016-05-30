using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IHasHealth
  {
    int MaxHealth { get; }
    int Health { get; }
    bool IsDead { get; }

    void Heal(int amountToHeal);
    void TakeDamage(int damage);
    void TakeDamage(int damage, Vector2 knockback, Vector2 knockbackSource);
    void Kill();
  }
}
