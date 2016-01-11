using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class Player : StatusEffectableCharacter
  {
    [InjectLocal] private FartAimLean FartAimLean { get; set; }

    [InjectLocal] public override IMovable Movement { get; protected set; }
    [InjectLocal] public override IHasHealth Health { get; protected set; }
    [InjectLocal] public IScoreKeeper Score { get; private set; }
    [InjectLocal] public IFartInfoProvider FartInfo { get; private set; }
    [InjectLocal] public IHasHealthContainers HealthContainers { get; private set; }
  }
}
