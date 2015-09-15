using UnityEngine;

public class FartWithStatusEffect : BasicFart, IStatusEffectAttacher
{
	[SerializeField]
	protected IStatusEffect statusEffect = null;

	public void AttachStatusEffect(IStatusEffectable statusEffectableObject) => statusEffectableObject.AddStatusEffect(statusEffect);

	protected override void DamageEnemy(ICharacter enemy)
	{
		base.DamageEnemy(enemy);

		var statusEffectableEnemy = enemy as IStatusEffectable;

		if (statusEffectableEnemy != null)
			AttachStatusEffect(statusEffectableEnemy);
	}
}