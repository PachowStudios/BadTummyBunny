using System;
using UnityEngine;

public abstract class BaseStatusEffect : MonoBehaviour, IStatusEffect
{
	public event Action<IStatusEffect> Deactivated;

	[SerializeField]
	private string statusEffectName = "Status Effect";

	public string StatusEffectName => statusEffectName;

	public ICharacter AffectedCharacter { get; private set; }

	public virtual void Activate(ICharacter affectedCharacter) => AffectedCharacter = affectedCharacter;

	public abstract void UpdateEffect();

	protected virtual void Deactivate() => Deactivated?.Invoke(this);
}