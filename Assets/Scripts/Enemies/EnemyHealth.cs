using UnityEngine;

[AddComponentMenu("Enemy/Health")]
public class EnemyHealth : BaseHasHealth
{
	[Header("Health")]
	[SerializeField]
	protected int maxHealth = 4;
	[SerializeField]
	protected Color flashColor = new Color(1f, 0.47f, 0.47f, 1f);
	[SerializeField]
	protected float flashLength = 0.25f;

	[Header("Components")]
	[SerializeField]
	protected SpriteRenderer spriteRenderer = null;

	private ICharacter thisEnemy = null;

	public override int Health
	{
		get { return health; }
		protected set
		{
			health = Mathf.Clamp(value, 0, MaxHealth);
			RaiseHealthChanged(health);
			CheckDeath();
		}
	}

	public override int MaxHealth => maxHealth;

	protected ICharacter ThisEnemy => this.GetInterfaceIfNull(ref thisEnemy);

	public override void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection)
	{
		if (IsDead || damage <= 0f) return;

		Health -= damage;

		if (!IsDead)
		{
			ThisEnemy.Movement.ApplyKnockback(knockback, knockbackDirection);
			spriteRenderer.color = flashColor;
			Wait.ForSeconds(flashLength, ResetColor);
		}
	}

	public override void Kill()
	{
		IsDead = true;
		ExplodeEffect.Instance.Explode(transform, ThisEnemy.Movement.Velocity, spriteRenderer.sprite);
		Destroy(gameObject);
	}

	protected virtual void Awake()
	{
		health = MaxHealth;
	}

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == Tags.Killzone)
			Kill();
	}

	protected virtual void OnTriggerStay2D(Collider2D other) => OnTriggerEnter2D(other);

	protected virtual void CheckDeath()
	{
		if (Health <= 0f)
			Kill();
	}

	protected virtual void ResetColor() => spriteRenderer.color = Color.white;
}