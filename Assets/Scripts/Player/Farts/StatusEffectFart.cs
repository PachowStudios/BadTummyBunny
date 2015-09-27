using UnityEngine;

[AddComponentMenu("Player/Farts/Status Effect Fart")]
public class StatusEffectFart : BasicFart, IStatusEffectAttacher
{
	[Header("Status Effects")]
	[SerializeField]
	protected MonoBehaviour statusEffect = null;

	public void AttachStatusEffect(IStatusEffectable statusEffectableObject) 
		=> statusEffectableObject.AddStatusEffect((IStatusEffect)this.statusEffect);

	protected override void DamageEnemy(ICharacter enemy)
	{
		base.DamageEnemy(enemy);

		var statusEffectableEnemy = enemy as IStatusEffectable;

		if (statusEffectableEnemy != null)
			AttachStatusEffect(statusEffectableEnemy);
	}
}