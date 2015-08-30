using System;
using UnityEngine;

[AddComponentMenu("Player/Health")]
public sealed class PlayerHealth : MonoBehaviour
{
	#region Events
	public event Action<int> HealthChanged = delegate { };
	public event Action<int> HeartContainersChanged = delegate { };
	#endregion

	#region Constants
	public const int HealthPerContainer = 4;
	#endregion

	#region Fields
	public int heartContainers = 3;
	public int carrotHealthRecharge = 1;
	public int falloutDamage = 1;
	public int damage = 4;
	public float damageRate = 3f;
	public float invincibilityPeriod = 2f;
	public Vector2 knockback = new Vector2(2f, 2f);

	public float fartRange = 10f;
	public Vector2 fartWidth = new Vector2(1f, 4f);
	[SerializeField]
	private Transform fartColliderTransform;

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
	#endregion

	#region Public Properties
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
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		fartCollider = fartColliderTransform.gameObject.AddComponent<PolygonCollider2D>();
		fartCollider.isTrigger = true;
		fartCollider.enabled = false;
		SetFartCollider();

		health = MaxHealth;
		damageTime = 1f / damageRate;
		damageTimer = damageTime;
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
		{
			UpdateInvincibilityFlash();

			if (PlayerControl.Instance.IsFarting)
			{
				damageTimer += Time.deltaTime;

				if (damageTimer >= damageTime)
				{
					DamageTargets();
					damageTimer = 0f;
				}
			}
		}
	}
	#endregion

	#region Internal Update Methods
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

	// Ewwwwwww fuck it only 9 hours left
	// I feel unclean for writing this
	private void DamageTargets()
	{
		Vector3 origin = fartColliderTransform.position;

		foreach (Enemy enemy in FindObjectsOfType<Enemy>())
		{
			if (enemy.collider2D != null)
			{
				fartCollider.enabled = true;

				if (fartCollider.OverlapPoint(enemy.collider2D.bounds.center))
				{
					RaycastHit2D linecast = Physics2D.Linecast(origin, enemy.collider2D.bounds.center, PlayerControl.Instance.CollisionLayers);
					
					if (linecast.collider == null)
						enemy.TakeDamageFromPlayer(damage);
				}

				fartCollider.enabled = false;
			}
		}
	}
	#endregion

	#region Internal Helper Methods
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

	private void SetFartCollider()
	{
		fartCollider.SetPath(0, new Vector2[] { fartColliderTransform.TransformPointLocal(new Vector2(0.75f, fartWidth.x / 2f)),
			                                      fartColliderTransform.TransformPointLocal(new Vector2(0.75f, -(fartWidth.x / 2f))),
			                                      fartColliderTransform.TransformPointLocal(new Vector2(-fartRange, (-fartWidth.y / 2f))),
			                                      fartColliderTransform.TransformPointLocal(new Vector2(-fartRange, fartWidth.y / 2f)) });
	}
	#endregion

	#region Public Methods
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
	#endregion
}
