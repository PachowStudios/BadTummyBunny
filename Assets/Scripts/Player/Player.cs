using UnityEngine;

[AddComponentMenu("Player/Player")]
[RequireComponent(typeof(IMovable), typeof(IHasHealth), typeof(IScoreKeeper))]
[RequireComponent(typeof(PlayerTriggers))]
public sealed class Player : StatusEffectableCharacter
{
	private PlayerControl movement = null;
	private PlayerHealth health = null;
	private PlayerScore score = null;
	private PlayerTriggers triggers = null;

	public static Player Instance { get; private set; }

	public override IMovable Movement => this.GetComponentIfNull(ref movement);
	public override IHasHealth Health => this.GetComponentIfNull(ref health);
	public IFartStatusProvider FartStatusProvider => this.GetComponentIfNull(ref movement);
	public IHasHealthContainers HealthContainers => this.GetComponentIfNull(ref health);
	public IScoreKeeper Score => this.GetComponentIfNull(ref score);
	public PlayerTriggers Triggers => this.GetComponentIfNull(ref triggers);

	private void Awake()
	{
		Instance = this;
	}
}