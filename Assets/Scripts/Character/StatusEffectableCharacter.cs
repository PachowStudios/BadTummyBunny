using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class StatusEffectableCharacter : BaseCharacter, IStatusEffectable
{
	protected List<IStatusEffect> statusEffects = new List<IStatusEffect>();

	public ReadOnlyCollection<IStatusEffect> StatusEffects => statusEffects.AsReadOnly();

	public ICharacter Character => this;

	public void AddStatusEffect(IStatusEffect newStatusEffect)
	{
		if (newStatusEffect == null) return;

		var instance = Instantiate(newStatusEffect as MonoBehaviour, transform.position, Quaternion.identity) as MonoBehaviour;

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