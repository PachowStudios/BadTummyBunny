using System;
using UnityEngine;

public abstract class BaseStatusEffect : MonoBehaviour, IStatusEffect
{
	public event Action<IStatusEffect> Deactivated;

	[SerializeField]
	private string statusEffectName = "Status Effect";

	public string StatusEffectName => statusEffectName;

	public IStatusEffectableCharacter AffectedCharacter { get; private set; }

	public virtual void Activate(IStatusEffectableCharacter affectedObject) => AffectedCharacter = affectedObject;

	public abstract void UpdateEffect();

	protected virtual void Deactivate() => Deactivated?.Invoke(this);
}