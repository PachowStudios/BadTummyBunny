using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class BurningStatusEffect : BaseStatusEffect<BurningStatusEffectSettings>
  {
    [Inject] private IStatusEffectView View { get; set; }

    [Inject] protected override BurningStatusEffectSettings Config { get; set; }

    private float DamageTimer { get; set; }
    private float DurationTimer { get; set; }
    private float Duration { get; set; }

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
        AffectedCharacter.Health.Damage(Config.Damage);
        DamageTimer = 0f;
      }

      DurationTimer += Time.deltaTime;

      if (DurationTimer >= Duration)
        Detach();
    }
  }
}
