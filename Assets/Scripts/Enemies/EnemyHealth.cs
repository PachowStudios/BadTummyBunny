using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Enemy/Health")]
  public class EnemyHealth : BaseHasHealth
  {
    [Header("Health")]
    [SerializeField] protected int maxHealth = 4;
    [SerializeField] protected Color flashColor = new Color(1f, 0.47f, 0.47f, 1f);
    [SerializeField] protected float flashLength = 0.25f;

    [Header("Components")]
    [SerializeField] protected SpriteRenderer spriteRenderer = null;

    private ICharacter thisEnemy;

    public override int Health
    {
      get { return this.health; }
      protected set
      {
        this.health = Mathf.Clamp(value, 0, MaxHealth);
        RaiseHealthChanged(this.health);
        CheckDeath();
      }
    }

    public override int MaxHealth => this.maxHealth;

    protected ICharacter ThisEnemy => this.GetInterfaceIfNull(ref this.thisEnemy);

    public override void Damage(int damage, Vector2 knockback, Vector2 knockbackDirection)
    {
      if (IsDead || damage <= 0f)
        return;

      Health -= damage;

      if (IsDead)
        return;

      ThisEnemy.Movement.ApplyKnockback(knockback, knockbackDirection);
      this.spriteRenderer.color = this.flashColor;
      Wait.ForSeconds(this.flashLength, ResetColor);
    }

    public override void Kill()
    {
      IsDead = true;
      ExplodeEffect.Instance.Explode(transform, ThisEnemy.Movement.Velocity, this.spriteRenderer.sprite);
      Destroy(gameObject);
    }

    protected virtual void Awake() 
      => this.health = MaxHealth;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == Tags.Killzone)
        Kill();
    }

    protected virtual void OnTriggerStay2D(Collider2D other) 
      => OnTriggerEnter2D(other);

    protected virtual void CheckDeath()
    {
      if (Health <= 0f)
        Kill();
    }

    protected virtual void ResetColor() 
      => this.spriteRenderer.color = Color.white;
  }
}
