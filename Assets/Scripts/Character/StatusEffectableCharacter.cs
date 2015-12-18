using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public abstract class StatusEffectableCharacter : MonoBehaviour, ICharacter, IStatusEffectable
  {
    public abstract IMovable Movement { get; }
    public abstract IHasHealth Health { get; }

    protected readonly List<IStatusEffect> statusEffects = new List<IStatusEffect>();

    public ReadOnlyCollection<IStatusEffect> StatusEffects => this.statusEffects.AsReadOnly();

    public void AddStatusEffect(IStatusEffect newStatusEffect)
    {
      if (newStatusEffect == null || this.statusEffects.Any(e => e.StatusEffectName == newStatusEffect.StatusEffectName))
        return;

      var instance = (MonoBehaviour)Instantiate((MonoBehaviour)newStatusEffect, Movement.CenterPoint, Quaternion.identity);

      instance.name = newStatusEffect.StatusEffectName;
      instance.transform.parent = transform;

      var statusEffectInstance = (IStatusEffect)instance;

      statusEffectInstance.Deactivated += RemoveStatusEffect;
      statusEffectInstance.Activate(this);
      this.statusEffects.Add(statusEffectInstance);
    }

    public void RemoveStatusEffect(IStatusEffect oldStatusEffect)
    {
      if (oldStatusEffect == null)
        return;

      oldStatusEffect.Deactivated -= RemoveStatusEffect;
      this.statusEffects.Remove(oldStatusEffect);
      (oldStatusEffect as MonoBehaviour)?.DestroyGameObject();
    }
  }
}
