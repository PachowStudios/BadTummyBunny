using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class EnemyHealth : BaseHasHealth, IDisposable,
    IHandles<CharacterKillzoneTriggeredMessage>
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      public int MaxHealth = 4;
      public Color FlashColor = new Color(1f, 0.47f, 0.47f, 1f);
      public float FlashLength = 0.25f;
    }

    private int health;

    [InjectLocal] private Settings Config { get; set; }
    [InjectLocal] private EnemyView View { get; set; }
    [InjectLocal] private IMovable Movement { get; set; }
    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject] private ExplodeEffect ExplodeEffect { get; set; }

    public override int Health
    {
      get { return this.health; }
      protected set
      {
        this.health = Mathf.Clamp(value, 0, MaxHealth);
        CheckDeath();
      }
    }

    public sealed override int MaxHealth => Config.MaxHealth;

    public EnemyHealth()
    {
      this.health = MaxHealth;
    }

    [PostInject]
    private void Initialize()
      => LocalEventAggregator.Subscribe(this);

    public void Dispose()
      => View.Dispose();

    public override void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection)
    {
      if (IsDead || damage <= 0f)
        return;

      Health -= damage;

      if (IsDead)
        return;

      Movement.ApplyKnockback(knockback, knockbackDirection);
      View.SpriteRenderer.Flash(Config.FlashColor, Config.FlashLength);
    }

    public override void Kill()
    {
      IsDead = true;
      ExplodeEffect.Explode(View.Transform, Movement.Velocity, View.SpriteRenderer.sprite);
      Dispose();
    }

    protected virtual void CheckDeath()
    {
      if (Health <= 0f)
        Kill();
    }

    public void Handle(CharacterKillzoneTriggeredMessage message)
      => Kill();
  }
}
