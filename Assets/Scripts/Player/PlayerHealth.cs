using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player/Health")]
  public sealed class PlayerHealth : BaseHasHealth, IHasHealthContainers,
    IHandles<PlayerCarrotTriggeredMessage>,
    IHandles<PlayerEnemyTriggeredMessage>,
    IHandles<PlayerRespawnPointTriggeredMessage>,
    IHandles<PlayerKillzoneTriggeredMessage>
  {
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

    [Inject(Tags.Player)]
    private IMovable Movement { get; set; }

    [Inject]
    private IEventAggregator EventAggregator { get; set; }

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
        EventAggregator.Publish(new PlayerHealthContainersChangedMessage(value));
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

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public override void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection)
    {
      if (IsInvincible || IsDead || damage <= 0f)
        return;

      Health -= damage;

      if (!IsDead)
        Wait.ForSeconds(0.1f, () => Movement.ApplyKnockback(knockback, knockbackDirection));
    }

    public override void Kill() { }

    protected override void RaiseHealthChanged(int newHealth)
    {
      base.RaiseHealthChanged(newHealth);

      EventAggregator.Publish(new PlayerHealthChangedMessage(newHealth));
    }

    private void Awake()
    {
      this.health = MaxHealth;
      this.lastHitTime = Time.time - this.invincibilityPeriod;
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

    private void HealFromCarrot()
      => Heal(this.carrotHealthRecharge);

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

    public void Handle(PlayerCarrotTriggeredMessage message)
      => HealFromCarrot();

    public void Handle(PlayerEnemyTriggeredMessage message)
      => Damage(message.Enemy);

    public void Handle(PlayerRespawnPointTriggeredMessage message)
      => SetRespawnPoint(message.RespawnPoint);

    public void Handle(PlayerKillzoneTriggeredMessage message)
      => Respawn();
  }
}
