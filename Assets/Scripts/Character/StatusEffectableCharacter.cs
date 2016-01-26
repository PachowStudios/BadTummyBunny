using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class StatusEffectableCharacter<TMovement, THealth> : Facade, IStatusEffectable
    where TMovement : IMovable
    where THealth : IHasHealth
  {
    private readonly List<IStatusEffect> statusEffects = new List<IStatusEffect>();

    [Inject] private IFactory<StatusEffectType, IStatusEffect> StatusEffectFactory { get; set; }

    [Inject] public IView View { get; private set; }

    public abstract TMovement Movement { get; protected set; }
    public abstract THealth Health { get; protected set; }

    IMovable ICharacter.Movement => Movement;
    IHasHealth ICharacter.Health => Health;

    public IEnumerable<IStatusEffect> StatusEffects => this.statusEffects;

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
      => this.statusEffects
        .Remove(e => e.Type == type)
        .Detach();

    public override void Tick()
    {
      base.Tick();

      foreach (var statusEffect in this.statusEffects)
        statusEffect.Tick();
    }
  }
}
