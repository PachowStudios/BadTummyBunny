using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IEnemy : ICharacter
  {
    string Name { get; }
    EnemyType Type { get; }
    int ContactDamage { get; }
    Vector2 ContactKnockback { get; }
  }
}
