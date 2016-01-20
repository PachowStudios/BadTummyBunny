using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class BurningStatusEffect : BaseStatusEffect<BurningStatusEffectSettings>
  {
    private float damageTimer;
    private float durationTimer;
    private float duration;

    [Inject] private IStatusEffectView View { get; set; }

    [Inject] protected override BurningStatusEffectSettings Config { get; set; }

    public override void Attach(IStatusEffectable affectectedCharacter)
    {
      base.Attach(affectectedCharacter);

      this.duration = Random.Range(Config.MinDuration, Config.MaxDuration);
      View.Attach(affectectedCharacter);
    }

    public override void Detach()
      => View.Detach();

    public override void Tick()
    {
      base.Tick();

      this.damageTimer += Time.deltaTime;

      if (this.damageTimer >= Config.TimePerDamage)
      {
        AffectedCharacter.Health.Damage(Config.Damage);
        this.damageTimer = 0f;
      }

      this.durationTimer += Time.deltaTime;

      if (this.durationTimer >= this.duration)
        Detach();
    }
  }
}
