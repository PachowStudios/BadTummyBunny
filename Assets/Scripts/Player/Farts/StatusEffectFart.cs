using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StatusEffectFart : Fart
  {
    [InstallerSettings]
    public class StatusEffectSettings : Settings
    {
      public StatusEffectType StatusEffectType;
    }

    [Inject] private StatusEffectSettings Config { get; set; }

    public void AttachStatusEffect(IStatusEffectable target)
      => target.AddStatusEffect(Config.StatusEffectType);

    protected override void DamageEnemy(IEnemy enemy)
    {
      base.DamageEnemy(enemy);

      var statusEffectableEnemy = enemy as IStatusEffectable;

      if (statusEffectableEnemy != null)
        AttachStatusEffect(statusEffectableEnemy);
    }
  }
}
