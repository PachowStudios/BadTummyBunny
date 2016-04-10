using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class Enemy : StatusEffectableCharacter, IEnemy
  {
    public EnemyMovement Movement { get; }
    public EnemyHealth Health { get; }

    public string Name => Config.Name;
    public EnemyType Type => Config.Prefab.Type;
    public int ContactDamage => Config.ContactDamage;
    public Vector2 ContactKnockback => Config.ContactKnockback;

    private EnemySettings Config { get; }

    public Enemy(EnemySettings config, IView view, EnemyMovement movement, EnemyHealth health)
      : base(view, movement, health)
    {
      Config = config;
      Movement = movement;
      Health = health;
    }
  }
}
