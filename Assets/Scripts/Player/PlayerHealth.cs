using System;
using UnityEngine;

[AddComponentMenu("Player/Health")]
public sealed class PlayerHealth : BaseHasHealth, IHasHealthContainers
{
	public event Action<int> HealthContainersChanged;

	[Header("Health")]
	[SerializeField]
	private int healthContainers = 3;
	[SerializeField]
	private int carrotHealthRecharge = 1;
	[SerializeField]
	private int falloutDamage = 1;
	[SerializeField]
	private float invincibilityPeriod = 2f;

	[Header("Components")]
	[SerializeField]
	private SpriteRenderer spriteRenderer = null;

	private bool invincible = false;
	private float lastHitTime = 0f;
	private float flashTimer = 0f;
	private float flashTime = 0.25f;
	private float smoothFlashTime;

	private RespawnPoint respawnPoint = null;

	public int HealthContainers
	{
		get { return healthContainers; }
		set
		{
			if (healthContainers == value) return;

			if (healthContainers < value)
				health = Mathf.Min(health + ((value - healthContainers) * HealthPerContainer), value * HealthPerContainer);
			else if (healthContainers > value)
				health = Mathf.Min(health, value * HealthPerContainer);

			healthContainers = value;
			HealthContainersChanged?.Invoke(healthContainers);
			RaiseHealthChanged(health);
		}
	}

	public override int Health
	{
		get { return health; }
		protected set
		{
			if (value < health) lastHitTime = Time.time;

			health = Mathf.Clamp(value, 0, MaxHealth);
			RaiseHealthChanged(health);
			CheckDeath();
		}
	}

	public int HealthPerContainer => 4;
	public override int MaxHealth => HealthContainers * HealthPerContainer;

	public override void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection)
	{
		if (invincible || IsDead || damage <= 0f) return;

		Health -= damage;

		if (!IsDead)
			Wait.ForSeconds(0.1f, () => Player.Instance.Movement.ApplyKnockback(knockback, knockbackDirection));
	}

	public void Damage(Enemy enemy) => Damage(enemy.ContactDamage, enemy.ContactKnockback, enemy.Movement.Direction);

	public override void Kill()
	{

	}

	private void Awake()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		health = MaxHealth;
		lastHitTime = Time.time - invincibilityPeriod;
	}

	private void Start()
	{
		Player.Instance.Triggers.CarrotTriggered += HealFromCarrot;
		Player.Instance.Triggers.EnemyTriggered += Damage;
		Player.Instance.Triggers.KillzoneTriggered += Respawn;
		Player.Instance.Triggers.RespawnPointTriggered += SetRespawnPoint;
	}

	private void OnDestroy()
	{
		Player.Instance.Triggers.CarrotTriggered -= HealFromCarrot;
		Player.Instance.Triggers.EnemyTriggered -= Damage;
		Player.Instance.Triggers.KillzoneTriggered -= Respawn;
		Player.Instance.Triggers.RespawnPointTriggered -= SetRespawnPoint;
	}

	private void Update()
	{
		if (!IsDead)
			UpdateInvincibilityFlash();
	}

	private void UpdateInvincibilityFlash()
	{
		invincible = Time.time <= lastHitTime + invincibilityPeriod;

		if (invincible)
		{
			flashTimer += Time.deltaTime;
			smoothFlashTime = Mathf.Lerp(smoothFlashTime, 0.05f, 0.025f);

			if (flashTimer > smoothFlashTime)
			{
				AlternateRenderersEnabled();
				flashTimer = 0f;
			}
		}
		else
		{
			SetRenderersEnabled(true);
			smoothFlashTime = flashTime;
		}
	}

	private void HealFromCarrot(Carrot carrot)
	{
		Heal(carrotHealthRecharge);
	}

	private void CheckDeath()
	{
		if (Health <= 0 && !IsDead)
		{
			IsDead = true;

			GameMenu.Instance.ShowGameOver();
			SetRenderersEnabled(false);
			collider2D.enabled = false;
			ExplodeEffect.Instance.Explode(transform, Player.Instance.Movement.Velocity, spriteRenderer.sprite);
			//TODO: Come back to this later!
			//Player.Instance.Movement.DisableInput();
		}
	}

	private void Respawn()
	{
		if (IsDead) return;

		Health -= falloutDamage;

		if (!IsDead)
			transform.position = respawnPoint?.Location ?? Vector3.zero;
	}

	private void SetRespawnPoint(RespawnPoint newRespawnPoint)
	{
		if (newRespawnPoint == null) return;

		if (respawnPoint != null)
		{
			if (respawnPoint == newRespawnPoint) return;

			respawnPoint.Deactivate();
		}

		newRespawnPoint.Activate();
		respawnPoint = newRespawnPoint;
	}

	private void SetRenderersEnabled(bool enabled) => spriteRenderer.enabled = enabled;

	private void AlternateRenderersEnabled() => spriteRenderer.enabled = !spriteRenderer.enabled;
}