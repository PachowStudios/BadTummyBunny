using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IHasHealth
  {
    int MaxHealth { get; }
    int Health { get; }
    bool IsDead { get; }

    void Heal(int amountToHeal);
    void Damage(int damage);
    void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection);
    void Kill();
  }
}
