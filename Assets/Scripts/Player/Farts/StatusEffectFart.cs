using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StatusEffectFart : BasicFart, IStatusEffectAttacher
  {
    [InstallerSettings]
    public class Settings : BasicSettings
    {
      public StatusEffectType StatusEffectType;
    }

    [Inject] private Settings Config { get; set; }

    public void AttachStatusEffect(IStatusEffectable target)
      => target.AddStatusEffect(Config.StatusEffectType);

    protected override void DamageEnemy(ICharacter enemy)
    {
      base.DamageEnemy(enemy);

      var statusEffectableEnemy = enemy as IStatusEffectable;

      if (statusEffectableEnemy != null)
        AttachStatusEffect(statusEffectableEnemy);
    }
  }
}
