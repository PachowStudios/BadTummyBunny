using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Status Effects/Burning")]
  public class BurningStatusEffect : BaseStatusEffect
  {
    [InstallerSettings]
    public class Settings : BaseSettings
    {
      public int Damage = 1;
      public float TimePerDamage = 0.5f;
      public float MinDuration = 2f;
      public float MaxDuration = 4f;
    }

    private float damageTimer;
    private float durationTimer;
    private float duration;

    [Inject] private Settings Config { get; set; }
    [Inject] private IStatusEffectView View { get; set; }

    public override void Attach(IStatusEffectable affectectedCharacter)
    {
      base.Attach(affectectedCharacter);

      this.duration = Random.Range(Config.MinDuration, Config.MaxDuration);
      View.Attach(affectectedCharacter);
    }

    public override void Dispose()
      => View.Detach();

    public override void Tick()
    {
      base.Tick();

      this.damageTimer += Time.deltaTime;

      if (this.damageTimer >= Config.TimePerDamage)
      {
        AffectedCharacter?.Health.Damage(Config.Damage);
        this.damageTimer = 0f;
      }

      this.durationTimer += Time.deltaTime;

      if (this.durationTimer >= this.duration)
        Dispose();
    }
  }
}
