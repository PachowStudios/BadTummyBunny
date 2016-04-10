using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class StatusEffectableCharacter : Facade, IStatusEffectable
  {
    private readonly List<IStatusEffect> statusEffects = new List<IStatusEffect>();

    IMovable ICharacter.Movement => Movement;
    IHasHealth ICharacter.Health => Health;

    public IView View { get; }
    public IEnumerable<IStatusEffect> StatusEffects => this.statusEffects;

    private IMovable Movement { get; }
    private IHasHealth Health { get; }

    [Inject] private IFactory<StatusEffectType, IStatusEffect> StatusEffectFactory { get; set; }

    protected StatusEffectableCharacter(IView view, IMovable movement, IHasHealth health)
    {
      View = view;
      Movement = movement;
      Health = health;
    }

    public bool HasStatusEffect(StatusEffectType type)
      => this.statusEffects.Any(e => e.Type == type);

    public void AddStatusEffect(StatusEffectType type)
    {
      if (HasStatusEffect(type))
        return;

      var statusEffect = StatusEffectFactory.Create(type);

      statusEffect.Attach(this);
      this.statusEffects.Add(statusEffect);
    }

    public void RemoveStatusEffect(StatusEffectType type)
      => this.statusEffects.RemoveSingle(e => e.Type == type)?.Detach();

    public override void Tick()
    {
      base.Tick();

      foreach (var statusEffect in this.statusEffects)
        statusEffect.Tick();
    }
  }
}
