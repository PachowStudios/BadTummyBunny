using PachowStudios.Assertions;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class PlayerHealth : BaseHasHealth, IInitializable,
    IHandles<CharacterKillzoneTriggeredMessage>,
    IHandles<PlayerCarrotCollectedMessage>,
    IHandles<PlayerEnemyCollidedMessage>,
    IHandles<PlayerRespawnPointActivatedMessage>
  {
    private int health;
    private int healthContainers;

    private float lastHitTime;

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

        if (Health <= 0)
          Kill();
      }
    }

    public int HealthPerContainer => 4;
    public override int MaxHealth => HealthContainers * HealthPerContainer;
    public bool IsInvincible => this.lastHitTime + Config.InvincibilityPeriod >= Time.time;

    private PlayerHealthSettings Config { get; }
    private PlayerView View { get; }

    [InjectLocal] private IMovable Movement { get; set; }
    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject(BindingIds.Global)] private IEventAggregator EventAggregator { set; get; }
    [Inject] private ExplodeEffect ExplodeEffect { get; set; }

    private RespawnPoint RespawnPoint { get; set; }

    public PlayerHealth(PlayerHealthSettings config, PlayerView view)
    {
      Config = config;
      View = view;

      this.healthContainers = Config.HealthContainers;
      this.health = MaxHealth;
      this.lastHitTime = Time.time - Config.InvincibilityPeriod;
    }

    [PostInject]
    private void PostInject()
    {
      LocalEventAggregator.Subscribe(this);
      EventAggregator.Subscribe(this);
    }

    public void Initialize()
      => RaiseHealthContainersChanged();

    public override void TakeDamage(int damage, Vector2 knockback, Vector2 knockbackSource)
    {
      damage.Should().BeGreaterThan(0, "because the player cannot take negative damage");

      if (IsInvincible || IsDead)
        return;

      Health -= damage;

      if (!IsDead)
        Wait.ForSeconds(0.1f, () => Movement.ApplyKnockback(knockback, knockbackSource));
    }

    public override void Kill()
    {
      if (IsDead)
        return;

      IsDead = true;

      EventAggregator.Publish(new PlayerDiedMessage());
      ExplodeEffect.Explode(View.Transform, Movement.Velocity, View.SpriteRenderer.sprite);
      View.SetRenderersEnabled(false);
      Movement.Disable();
    }

    private void TakeDamage(IEnemy enemy)
      => TakeDamage(enemy.ContactDamage, enemy.ContactKnockback, enemy.View.CenterPoint);

    private void HealFromCarrot()
      => Heal(Config.CarrotHealthRecharge);

    private void Respawn()
    {
      if (IsDead)
        return;

      Health -= Config.FalloutDamage;

      if (!IsDead)
        View.Transform.position = RespawnPoint?.Location ?? Vector3.zero;
    }

    private void SetRespawnPoint(RespawnPoint newRespawnPoint)
    {
      if (newRespawnPoint == null
          || RespawnPoint == newRespawnPoint)
        return;

      RespawnPoint?.Deactivate();
      newRespawnPoint.Activate();
      RespawnPoint = newRespawnPoint;
    }

    private void RaiseHealthChanged()
      => EventAggregator.Publish(new PlayerHealthChangedMessage(Health));

    private void RaiseHealthContainersChanged()
    {
      EventAggregator.Publish(new PlayerHealthContainersChangedMessage(HealthContainers, HealthPerContainer));
      RaiseHealthChanged();
    }

    public void Handle(CharacterKillzoneTriggeredMessage message)
      => Respawn();

    public void Handle(PlayerCarrotCollectedMessage message)
      => HealFromCarrot();

    public void Handle(PlayerEnemyCollidedMessage message)
      => TakeDamage(message.Enemy);

    public void Handle(PlayerRespawnPointActivatedMessage message)
      => SetRespawnPoint(message.RespawnPoint);
  }
}
