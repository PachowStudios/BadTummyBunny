using PachowStudios.Assertions;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class EnemyHealth : BaseHasHealth,
    IHandles<CharacterKillzoneTriggeredMessage>
  {
    private int health;

    private EnemyHealthSettings Config { get; }
    private EnemyView View { get; }

    [InjectLocal] private IMovable Movement { get; set; }
    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject] private ExplodeEffect ExplodeEffect { get; set; }

    public sealed override int Health
    {
      get { return this.health; }
      protected set
      {
        this.health = value.Clamp(0, MaxHealth);

        if (Health <= 0f)
          Kill();
      }
    }

    public sealed override int MaxHealth => Config.MaxHealth;

    public EnemyHealth(EnemyHealthSettings config, EnemyView view)
    {
      Config = config;
      View = view;

      Health = MaxHealth;
    }

    [PostInject]
    private void PostInject()
      => LocalEventAggregator.Subscribe(this);

    public override void TakeDamage(int damage, Vector2 knockback, Vector2 knockbackDirection)
    {
      damage.Should().BeGreaterThan(0, "because an enemy cannot take negative damage");

      if (IsDead)
        return;

      Health -= damage;
      LocalEventAggregator.Publish(new CharacterTookDamageMessage(damage));

      if (IsDead)
        return;

      Movement.ApplyKnockback(knockback, knockbackDirection);
      View.SpriteRenderer.Flash(Config.FlashColor, Config.FlashLength);
    }

    public override void Kill()
    {
      if (IsDead)
        return;

      IsDead = true;
      ExplodeEffect.Explode(View.Transform, Movement.Velocity, View.SpriteRenderer.sprite);
      View.Dispose();
    }

    public void Handle(CharacterKillzoneTriggeredMessage message)
      => Kill();
  }
}
