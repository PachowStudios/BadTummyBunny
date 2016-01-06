using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class Enemy : StatusEffectableCharacter, IEnemy
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      [Header("Definition")]
      public string Name = "New Enemy";
      public EnemyView Prefab;

      [Header("Options")]
      public int ContactDamage = 1;
      public Vector2 ContactKnockback = new Vector2(2f, 1f);

      [Header("Component Settings")]
      public EnemyMovement.MovementSettings Movement;
      public EnemyHealth.Settings Health;
    }

    [InjectLocal] private Settings Config { get; set; }

    [InjectLocal] public override IMovable Movement { get; protected set; }
    [InjectLocal] public override IHasHealth Health { get; protected set; }

    public int ContactDamage => Config.ContactDamage;
    public Vector2 ContactKnockback => Config.ContactKnockback;
  }
}
