using UnityEngine;

[AddComponentMenu("Status Effects/Burning")]
public class BurningStatusEffect : BaseStatusEffect
{
	[Header("Options")]
	[SerializeField]
	protected int damage = 1;
	[SerializeField]
	protected float timePerDamage = 0.5f;
	[SerializeField]
	protected float minDuration = 2f;
	[SerializeField]
	protected float maxDuration = 4f;

	[Header("Components")]
	[SerializeField]
	protected ParticleSystem particles = null;

	private float damageTimer = 0f;
	private float durationTimer = 0f;
	private float duration = 0f;

	protected override void UpdateEffect()
	{
		damageTimer += Time.deltaTime;

		if (damageTimer >= timePerDamage)
		{
			AffectedCharacter?.Health.Damage(damage);
			damageTimer = 0f;
		}

		durationTimer += Time.deltaTime;

		if (durationTimer >= duration)
			Deactivate();
	}

	protected override void OnActivate()
	{
		duration = Random.Range(minDuration, maxDuration);
		particles.Play();
	}

	protected override void OnDeactivate() => particles?.DetachAndDestroy();
}