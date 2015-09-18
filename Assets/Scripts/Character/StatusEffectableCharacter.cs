using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public abstract class StatusEffectableCharacter : MonoBehaviour, ICharacter, IStatusEffectable
{
	public abstract IMovable Movement { get; }
	public abstract IHasHealth Health { get; }

	protected List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public ReadOnlyCollection<IStatusEffect> StatusEffects => statusEffects.AsReadOnly();

	public void AddStatusEffect(IStatusEffect newStatusEffect)
	{
		if (newStatusEffect == null || statusEffects.Any(e => e.StatusEffectName == newStatusEffect.StatusEffectName))
			return;

		var instance = Instantiate(newStatusEffect as MonoBehaviour, Movement.CenterPoint, Quaternion.identity) as MonoBehaviour;

		instance.name = newStatusEffect.StatusEffectName;
		instance.transform.parent = transform;

		var statusEffectInstance = instance as IStatusEffect;

		statusEffectInstance.Deactivated += RemoveStatusEffect;
		statusEffectInstance.Activate(this);
		statusEffects.Add(statusEffectInstance);
	}

	public void RemoveStatusEffect(IStatusEffect oldStatusEffect)
	{
		if (oldStatusEffect == null) return;

		oldStatusEffect.Deactivated -= RemoveStatusEffect;
		statusEffects.Remove(oldStatusEffect);
		(oldStatusEffect as MonoBehaviour)?.DestroyGameObject();
	}
}