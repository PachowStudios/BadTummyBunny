using UnityEngine;

[AddComponentMenu("Status Effects/Burning")]
public class BurningStatusEffect : BaseStatusEffect
{
  [Header("Options")]
  [SerializeField] protected int damage = 1;
  [SerializeField] protected float timePerDamage = 0.5f;
  [SerializeField] protected float minDuration = 2f;
  [SerializeField] protected float maxDuration = 4f;

  [Header("Components")]
  [SerializeField] protected ParticleSystem particles = null;

  private float damageTimer;
  private float durationTimer;
  private float duration;

  protected override void UpdateEffect()
  {
    this.damageTimer += Time.deltaTime;

    if (this.damageTimer >= this.timePerDamage)
    {
      AffectedCharacter?.Health.Damage(this.damage);
      this.damageTimer = 0f;
    }

    this.durationTimer += Time.deltaTime;

    if (this.durationTimer >= this.duration)
      Deactivate();
  }

  protected override void OnActivate()
  {
    this.duration = Random.Range(this.minDuration, this.maxDuration);
    this.particles.Play();
  }

  protected override void OnDeactivate() 
    => this.particles?.DetachAndDestroy();
}
