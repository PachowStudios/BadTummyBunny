using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IEnemy : ICharacter
  {
    int ContactDamage { get; }
    Vector2 ContactKnockback { get; }
  }
}
