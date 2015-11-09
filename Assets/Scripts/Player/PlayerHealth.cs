using System;
using UnityEngine;

[AddComponentMenu("Player/Health")]
public sealed class PlayerHealth : BaseHasHealth, IHasHealthContainers
{
  public event Action<int> HealthContainersChanged;

  [Header("Health")]
  [SerializeField] private int healthContainers = 3;
  [SerializeField] private int carrotHealthRecharge = 1;
  [SerializeField] private int falloutDamage = 1;
  [SerializeField] private float invincibilityPeriod = 2f;

  [Header("Components")]
  [SerializeField] private SpriteRenderer spriteRenderer = null;

  private float lastHitTime;
  private float flashTimer;
  private float flashTime = 0.25f;
  private float smoothFlashTime;

  private RespawnPoint respawnPoint;

  public int HealthContainers
  {
    get { return this.healthContainers; }
    set
    {
      if (this.healthContainers == value)
        return;

      if (this.healthContainers < value)
        this.health = Mathf.Min(this.health + ((value - this.healthContainers) * HealthPerContainer), value * HealthPerContainer);
      else if (this.healthContainers > value)
        this.health = Mathf.Min(this.health, value * HealthPerContainer);

      this.healthContainers = value;
      HealthContainersChanged?.Invoke(this.healthContainers);
      RaiseHealthChanged(this.health);
    }
  }

  public override int Health
  {
    get { return this.health; }
    protected set
    {
      if (value < this.health)
        this.lastHitTime = Time.time;

      this.health = Mathf.Clamp(value, 0, MaxHealth);
      RaiseHealthChanged(this.health);
      CheckDeath();
    }
  }

  public int HealthPerContainer => 4;
  public override int MaxHealth => HealthContainers * HealthPerContainer;

  private bool IsInvincible => Time.time <= this.lastHitTime + this.invincibilityPeriod;
  private IMovable Movement => Player.Instance.Movement;

  public override void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection)
  {
    if (IsInvincible || IsDead || damage <= 0f)
      return;

    Health -= damage;

    if (!IsDead)
      Wait.ForSeconds(0.1f, () => Player.Instance.Movement.ApplyKnockback(knockback, knockbackDirection));
  }

  public override void Kill() { }

  private void Awake()
  {
    this.health = MaxHealth;
    this.lastHitTime = Time.time - this.invincibilityPeriod;
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
    if (IsInvincible)
    {
      this.flashTimer += Time.deltaTime;
      this.smoothFlashTime = Mathf.Lerp(this.smoothFlashTime, 0.05f, 0.025f);

      if (this.flashTimer > this.smoothFlashTime)
      {
        AlternateRenderersEnabled();
        this.flashTimer = 0f;
      }
    }
    else
    {
      SetRenderersEnabled(true);
      this.smoothFlashTime = this.flashTime;
    }
  }

  private void HealFromCarrot(Carrot carrot)
  {
    Heal(this.carrotHealthRecharge);
  }

  private void CheckDeath()
  {
    if (this.health > 0 || IsDead)
      return;

    IsDead = true;

    GameMenu.Instance.ShowGameOver();
    SetRenderersEnabled(false);
    ExplodeEffect.Instance.Explode(transform, Movement.Velocity, this.spriteRenderer.sprite);
    Movement.Disable();
  }

  private void Respawn()
  {
    if (IsDead)
      return;

    Health -= this.falloutDamage;

    if (!IsDead)
      transform.position = this.respawnPoint?.Location ?? Vector3.zero;
  }

  private void SetRespawnPoint(RespawnPoint newRespawnPoint)
  {
    if (newRespawnPoint == null)
      return;

    if (this.respawnPoint != null)
    {
      if (this.respawnPoint == newRespawnPoint)
        return;

      this.respawnPoint.Deactivate();
    }

    newRespawnPoint.Activate();
    this.respawnPoint = newRespawnPoint;
  }

  private void SetRenderersEnabled(bool enableRenderers)
    => this.spriteRenderer.enabled = enableRenderers;

  private void AlternateRenderersEnabled()
    => this.spriteRenderer.enabled = !this.spriteRenderer.enabled;

  private void Damage(IEnemy enemy)
    => Damage(enemy.ContactDamage, enemy.ContactKnockback, enemy.Movement.MovementDirection);
}
