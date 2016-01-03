using System;
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
    [Serializable, InstallerSettings]
    public class Settings
    {
      public int HealthContainers = 3;
      public int CarrotHealthRecharge = 1;
      public int FalloutDamage = 1;
      public float InvincibilityPeriod = 2f;
    }

    private const float FlashTime = 0.25f;

    private float lastHitTime;
    private float flashTimer;
    private float smoothFlashTime;

    private RespawnPoint respawnPoint;

    [Inject] private Settings Config { get; set; }
    [Inject] private PlayerView View { get; set; }
    [Inject] private IMovable Movement { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; }

    public int HealthContainers
    {
      get { return Config.HealthContainers; }
      set
      {
        if (Config.HealthContainers == value)
          return;

        if (Config.HealthContainers < value)
          this.health = Mathf.Min(this.health + ((value - Config.HealthContainers) * HealthPerContainer), value * HealthPerContainer);
        else if (Config.HealthContainers > value)
          this.health = Mathf.Min(this.health, value * HealthPerContainer);

        Config.HealthContainers = value;
        RaiseHealthContainersChanged(value);
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

    private bool IsInvincible => Time.time <= this.lastHitTime + Config.InvincibilityPeriod;

    public PlayerHealth()
    {
      this.health = MaxHealth;
      this.lastHitTime = Time.time - Config.InvincibilityPeriod;
    }

    [PostInject]
    private void Initialize()
      => EventAggregator.Subscribe(this);

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
      if (this.health > 0 || IsDead)
        return;

      IsDead = true;

      GameMenu.Instance.ShowGameOver();
      ExplodeEffect.Instance.Explode(View.Transform, Movement.Velocity, View.SpriteRenderer.sprite);
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

    private void RaiseHealthChanged(int newHealth)
      => EventAggregator.Publish(new PlayerHealthChangedMessage(newHealth));

    private void RaiseHealthContainersChanged(int value)
    {
      EventAggregator.Publish(new PlayerHealthContainersChangedMessage(value));
      RaiseHealthChanged(Health);
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
