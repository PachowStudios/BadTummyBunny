using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class Enemy : StatusEffectableCharacter, IEnemy
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      public int ContactDamage = 1;
      public Vector2 ContactKnockback = new Vector2(2f, 1f);
    }

    [Inject] private Settings Config { get; set; }

    [Inject] public override IMovable Movement { get; protected set; }
    [Inject] public override IHasHealth Health { get; protected set; }

    public int ContactDamage => Config.ContactDamage;
    public Vector2 ContactKnockback => Config.ContactKnockback;
  }
}
