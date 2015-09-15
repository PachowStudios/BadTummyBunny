using UnityEngine;

[AddComponentMenu("Enemy/Enemy")]
[RequireComponent(typeof(IMovable), typeof(IHasHealth))]
public class Enemy : StatusEffectableCharacter, IEnemy
{
	[Header("Contact Damage")]
	[SerializeField]
	protected int contactDamage = 1;
	[SerializeField]
	protected Vector2 contactKnockback = new Vector2(2f, 1f);

	private EnemyMovement movement = null;
	private EnemyHealth health = null;

	public override IMovable Movement => this.GetComponentIfNull(ref movement);
	public override IHasHealth Health => this.GetComponentIfNull(ref health);

	public int ContactDamage => contactDamage;
	public Vector2 ContactKnockback => contactKnockback;
}
