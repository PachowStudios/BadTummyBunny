using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Enemy/Enemy")]
  [RequireComponent(typeof(IMovable), typeof(IHasHealth))]
  public class Enemy : StatusEffectableCharacter, IEnemy
  {
    [Header("Contact Damage")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private Vector2 contactKnockback = new Vector2(2f, 1f);

    private EnemyMovement movement;
    private EnemyHealth health;

    public override IMovable Movement => this.GetComponentIfNull(ref this.movement);
    public override IHasHealth Health => this.GetComponentIfNull(ref this.health);

    public int ContactDamage => this.contactDamage;
    public Vector2 ContactKnockback => this.contactKnockback;
  }
}
