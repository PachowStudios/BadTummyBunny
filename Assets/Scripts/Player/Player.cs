using UnityEngine;

[AddComponentMenu("Player/Player")]
[RequireComponent(typeof(IMovable), typeof(IHasHealth), typeof(IScoreKeeper))]
[RequireComponent(typeof(PlayerTriggers))]
public sealed class Player : StatusEffectableCharacter
{
  private PlayerControl movement;
  private PlayerHealth health;
  private PlayerScore score;
  private PlayerTriggers triggers;

  public static Player Instance { get; private set; }

  public override IMovable Movement => this.GetComponentIfNull(ref this.movement);
  public override IHasHealth Health => this.GetComponentIfNull(ref this.health);
  public IScoreKeeper Score => this.GetComponentIfNull(ref this.score);
  public PlayerTriggers Triggers => this.GetComponentIfNull(ref this.triggers);

  public IFartInfoProvider FartStatusProvider => (IFartInfoProvider)Movement;
  public IHasHealthContainers HealthContainers => (IHasHealthContainers)Health;

  private void Awake()
    => Instance = this;
}
