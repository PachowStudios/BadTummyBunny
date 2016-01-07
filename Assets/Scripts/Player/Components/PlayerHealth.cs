using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class PlayerHealth : BaseHasHealth, IHasHealthContainers, ITickable,
    IHandles<CharacterKillzoneTriggeredMessage>,
    IHandles<PlayerCarrotTriggeredMessage>,
    IHandles<PlayerEnemyTriggeredMessage>,
    IHandles<PlayerRespawnPointTriggeredMessage>
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      public int HealthContainers = 3;
      public int CarrotHealthRecharge = 1;
      public int FalloutDamage = 1;
      public float InvincibilityPeriod = 2f;
    }

    private const float FlashTime = 0.25f;

    private int health;
    private int healthContainers;

    private float lastHitTime;
    private float flashTimer;
    private float smoothFlashTime;

    private RespawnPoint respawnPoint;

    [InjectLocal] private Settings Config { get; set; }
    [InjectLocal] private PlayerView View { get; set; }
    [InjectLocal] private IMovable Movement { get; set; }
    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject] private IEventAggregator EventAggregator { set; get; }
    [Inject] private IGameMenu GameMenu { get; set; }
    [Inject] private ExplodeEffect ExplodeEffect { get; set; }

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
        RaiseHealthContainersChanged();
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
        RaiseHealthChanged();
        CheckDeath();
      }
    }

    public int HealthPerContainer => 4;
    public override int MaxHealth => HealthContainers * HealthPerContainer;

    private bool IsInvincible => this.lastHitTime + Config.InvincibilityPeriod >= Time.time;

    public PlayerHealth()
    {
      this.healthContainers = Config.HealthContainers;
      this.health = MaxHealth;
      this.lastHitTime = Time.time - Config.InvincibilityPeriod;
    }

    [PostInject]
    private void Initialize()
      => LocalEventAggregator.Subscribe(this);

    public void Tick()
    {
      if (!IsDead)
        UpdateInvincibilityFlash();
    }

    public override void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection)
    {
      if (IsInvincible || IsDead || damage <= 0f)
        return;

      Health -= damage;

      if (!IsDead)
        Wait.ForSeconds(0.1f, () => Movement.ApplyKnockback(knockback, knockbackDirection));
    }

    public override void Kill() { }

    private void UpdateInvincibilityFlash()
    {
      if (IsInvincible)
      {
        this.flashTimer += Time.deltaTime;
        this.smoothFlashTime = Mathf.Lerp(this.smoothFlashTime, 0.05f, 0.025f);

        if (this.flashTimer > this.smoothFlashTime)
        {
          View.AlternateRenderersEnabled();
          this.flashTimer = 0f;
        }
      }
      else
      {
        View.SetRenderersEnabled(true);
        this.smoothFlashTime = FlashTime;
      }
    }

    private void HealFromCarrot()
      => Heal(Config.CarrotHealthRecharge);

    private void CheckDeath()
    {
      if (Health > 0 || IsDead)
        return;

      IsDead = true;

      GameMenu.ShowGameOverScreen = true;
      ExplodeEffect.Explode(View.Transform, Movement.Velocity, View.SpriteRenderer.sprite);
      View.SetRenderersEnabled(false);
      Movement.Disable();
    }

    private void Respawn()
    {
      if (IsDead)
        return;

      Health -= Config.FalloutDamage;

      if (!IsDead)
        View.Transform.position = this.respawnPoint?.Location ?? Vector3.zero;
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

    private void Damage(IEnemy enemy)
      => Damage(enemy.ContactDamage, enemy.ContactKnockback, enemy.Movement.MovementDirection);

    private void RaiseHealthChanged()
      => EventAggregator.Publish(new PlayerHealthChangedMessage(Health));

    private void RaiseHealthContainersChanged()
    {
      EventAggregator.Publish(new PlayerHealthContainersChangedMessage(HealthContainers));
      RaiseHealthChanged();
    }

    public void Handle(CharacterKillzoneTriggeredMessage message)
      => Respawn();

    public void Handle(PlayerCarrotTriggeredMessage message)
      => HealFromCarrot();

    public void Handle(PlayerEnemyTriggeredMessage message)
      => Damage(message.Enemy);

    public void Handle(PlayerRespawnPointTriggeredMessage message)
      => SetRespawnPoint(message.RespawnPoint);
  }
}
