using UnityEngine;

[AddComponentMenu("Player/Player")]
[RequireComponent(typeof(IMovable), typeof(IHasHealth), typeof(IScoreKeeper))]
[RequireComponent(typeof(PlayerTriggers))]
public sealed class Player : BaseStatusEffectableCharacter
{
	private PlayerControl movement = null;
	private PlayerHealth health = null;
	private PlayerScore score = null;
	private PlayerTriggers triggers = null;

	public static Player Instance { get; private set; }

	public override IMovable Movement => this.GetComponentIfNull(ref movement);
	public override IHasHealth Health => this.GetComponentIfNull(ref health);
	public IScoreKeeper Score => this.GetComponentIfNull(ref score);
	public PlayerTriggers Triggers => this.GetComponentIfNull(ref triggers);

	public IFartStatusProvider FartStatusProvider => (IFartStatusProvider)Movement;
	public IHasHealthContainers HealthContainers => (IHasHealthContainers)Health;

	private void Awake() => Instance = this;
}