using System;
using UnityEngine;

public abstract class BaseStatusEffect : MonoBehaviour, IStatusEffect
{
	public event Action<IStatusEffect> Deactivated;

	[SerializeField]
	private string statusEffectName = "Status Effect";

	public string StatusEffectName => statusEffectName;

	public ICharacter AffectedCharacter { get; private set; }
	public bool IsActive { get; private set; }
	public bool IsDisposed { get; private set; }

	public void Update()
	{
		if (IsActive)
			UpdateEffect();
	}

	public void Activate(ICharacter affectedCharacter)
	{
		if (IsActive || IsDisposed)
			return;

		IsActive = true;
		AffectedCharacter = affectedCharacter;
		OnActivate();
	}

	protected void Deactivate()
	{
		if (!IsActive || IsDisposed)
			return;

		IsActive = false;
		IsDisposed = true;
		OnDeactivate();
		Deactivated?.Invoke(this);
	}

	protected virtual void UpdateEffect() { }
	protected virtual void OnActivate() { }
	protected virtual void OnDeactivate() { }

	private void OnDisable() => Deactivate();
}