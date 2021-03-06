﻿using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class Enemy : StatusEffectableCharacter<IMovable, EnemyHealth>, IEnemy
  {
    [InjectLocal] private EnemySettings Config { get; set; }

    [InjectLocal] public override IMovable Movement { get; protected set; }
    [InjectLocal] public override EnemyHealth Health { get; protected set; }

    public string Name => Config.Name;
    public EnemyType Type => Config.Prefab.Type;
    public int ContactDamage => Config.ContactDamage;
    public Vector2 ContactKnockback => Config.ContactKnockback;
  }
}
