namespace PachowStudios.BadTummyBunny
{
  public class StatusEffectFart : BaseFart
  {
    private StatusEffectFartSettings Config { get; }

    public StatusEffectFart(StatusEffectFartSettings config, FartView view)
      : base(config, view)
    {
      Config = config;
    }

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
