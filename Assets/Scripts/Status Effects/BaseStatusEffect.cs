using System;
using UnityEngine;

public abstract class BaseStatusEffect : MonoBehaviour, IStatusEffect
{
	public event Action<IStatusEffect> Deactivated;

	[SerializeField]
	private string statusEffectName = "Status Effect";

	public string StatusEffectName => statusEffectName;

	public IStatusEffectable AffectedObject { get; private set; }

	public virtual void Activate(IStatusEffectable affectedObject) => AffectedObject = affectedObject;
	public virtual void Deactivate() => Deactivated?.Invoke(this);

	public abstract void UpdateEffect();
}