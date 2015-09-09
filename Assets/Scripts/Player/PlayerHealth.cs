using System;
using UnityEngine;

[AddComponentMenu("Player/Health")]
public sealed class PlayerHealth : MonoBehaviour
{
	public event Action<int> HealthChanged = delegate { };
	public event Action<int> HeartContainersChanged = delegate { };

	public const int HealthPerContainer = 4;

	public int heartContainers = 3;
	public int carrotHealthRecharge = 1;
	public int falloutDamage = 1;
	public float invincibilityPeriod = 2f;
	public Vector2 knockback = new Vector2(2f, 2f);

	private int health;
	private bool dead = false;

	private float damageTime;
	private float damageTimer;

	private bool invincible = false;
	private float lastHitTime;
	private float flashTimer = 0f;
	private float flashTime = 0.25f;
	private float smoothFlashTime;

	private RespawnPoint respawnPoint;

	private SpriteRenderer spriteRenderer;
	private PolygonCollider2D fartCollider;

	public static PlayerHealth Instance { get; private set; }

	public int HeartContainers
	{
		get { return heartContainers; }
		set
		{
			if (heartContainers == value) return;

			if (heartContainers < value)
				health = Mathf.Min(health + ((value - heartContainers) * HealthPerContainer), value * HealthPerContainer);
			else if (heartContainers > value)
				health = Mathf.Min(health, value * HealthPerContainer);

			heartContainers = value;
			HeartContainersChanged(heartContainers);
			HealthChanged(health);
		}
	}

	public int MaxHealth
	{ get { return HeartContainers * HealthPerContainer; } }

	public int Health
	{
		get { return health; }
		set
		{
			if (value < health) lastHitTime = Time.time;

			health = Mathf.Clamp(value, 0, MaxHealth);
			HealthChanged(health);

			CheckDeath();
		}
	}

	public float HealthPercent
	{ get { return Mathf.Clamp((float)health / MaxHealth, 0f, 1f); } }

	public bool IsDead
	{ get { return dead; } }

	public Vector2 Knockback
	{ get { return knockback; } }

	private void Awake()
	{
		Instance = this;

		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		health = MaxHealth;
		lastHitTime = Time.time - invincibilityPeriod;
	}

	private void Start()
	{
		PlayerTriggers.Instance.EnemyTriggered += TakeDamage;
		PlayerTriggers.Instance.KillzoneTriggered += Respawn;
		PlayerTriggers.Instance.RespawnPointTriggered += SetRespawnPoint;
	}

	private void OnDestroy()
	{
		PlayerTriggers.Instance.EnemyTriggered -= TakeDamage;
		PlayerTriggers.Instance.KillzoneTriggered -= Respawn;
		PlayerTriggers.Instance.RespawnPointTriggered -= SetRespawnPoint;
	}

	private void Update()
	{
		if (!dead)
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

	private void CheckDeath()
	{
		if (Health <= 0 && !dead)
		{
			dead = true;

			StartCoroutine(GameMenu.Instance.ShowGameOver(0f));
			SetRenderersEnabled(false);
			collider2D.enabled = false;
			ExplodeEffect.Instance.Explode(transform, PlayerControl.Instance.Velocity, spriteRenderer.sprite);
			PlayerControl.Instance.DisableInput();
		}
	}

	private void Respawn()
	{
		if (dead) return;

		Health -= falloutDamage;

		if (!dead)
		{
			if (respawnPoint == null) transform.position = Vector3.zero;
			else transform.position = respawnPoint.Location;
		}
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

	private void SetRenderersEnabled(bool enabled)
	{
		spriteRenderer.enabled = enabled;
	}

	private void AlternateRenderersEnabled()
	{
		spriteRenderer.enabled = !spriteRenderer.enabled;
	}

	public void TakeDamage(Enemy enemy, int damage, Vector2 knockback)
	{
		if (invincible || dead) return;

		float knockbackDirection = (transform.position.x - enemy.transform.position.x).Sign();

		if (damage != 0f)
		{
			Health -= damage;

			if (!dead && !PlayerControl.Instance.IsFarting && knockback != default(Vector2))
				StartCoroutine(PlayerControl.Instance.ApplyKnockback(knockback, knockbackDirection));
		}
	}

	public void TakeDamage(Enemy enemy)
	{
		TakeDamage(enemy, enemy.damage, enemy.knockback);
	}

}
