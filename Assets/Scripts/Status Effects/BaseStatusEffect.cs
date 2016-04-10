namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseStatusEffect : IStatusEffect
  {
    public IStatusEffectable AffectedCharacter { get; private set; }

    public string Name => Config.Name;
    public StatusEffectType Type => Config.Type;

    private BaseStatusEffectSettings Config { get; }

    protected BaseStatusEffect(BaseStatusEffectSettings config)
    {
      Config = config;
    }

    public virtual void Attach(IStatusEffectable affectedCharacter)
      => AffectedCharacter = affectedCharacter;

    public virtual void Detach() { }

    public virtual void Tick() { }
  }
}
