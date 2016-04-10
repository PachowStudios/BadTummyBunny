using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class BurningStatusEffect : BaseStatusEffect
  {
    private float DamageTimer { get; set; }
    private float DurationTimer { get; set; }
    private float Duration { get; set; }

    private BurningStatusEffectSettings Config { get; }
    private IStatusEffectView View { get; }

    public BurningStatusEffect(BurningStatusEffectSettings config, IStatusEffectView view)
      : base(config)
    {
      View = view;
      Config = config;
    }

    public override void Attach(IStatusEffectable affectectedCharacter)
    {
      base.Attach(affectectedCharacter);

      Duration = Random.Range(Config.MinDuration, Config.MaxDuration);
      View.Attach(affectectedCharacter);
    }

    public override void Detach()
      => View.Detach();

    public override void Tick()
    {
      base.Tick();

      DamageTimer += Time.deltaTime;

      if (DamageTimer >= Config.TimePerDamage)
      {
        AffectedCharacter.Health.TakeDamage(Config.Damage);
        DamageTimer = 0f;
      }

      DurationTimer += Time.deltaTime;

      if (DurationTimer >= Duration)
        Detach();
    }
  }
}
