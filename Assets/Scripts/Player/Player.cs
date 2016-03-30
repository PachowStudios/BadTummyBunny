using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class Player : StatusEffectableCharacter<PlayerMovement, PlayerHealth>
  {
    [InjectLocal] public override PlayerMovement Movement { get; protected set; }
    [InjectLocal] public override PlayerHealth Health { get; protected set; }

    [InjectLocal] public IFartInfoProvider FartInfo { get; private set; }
    [InjectLocal] public IHasHealthContainers HealthContainers { get; private set; }

    [InjectLocal] private FartAimLean FartAimLean { get; set; }
  }
}
