using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class Player : StatusEffectableCharacter
  {
    [Serializable, InstallerSettings]
    public class Settings
    {
      public PlayerMovement.Settings Movement;
      public PlayerHealth.Settings Health;
    }

    [Inject] public override IMovable Movement { get; protected set; }
    [Inject] public override IHasHealth Health { get; protected set; }
    [Inject] public IScoreKeeper Score { get; private set; }
    [Inject] public IFartInfoProvider FartStatusProvider { get; private set; }
    [Inject] public IHasHealthContainers HealthContainers { get; private set; }
  }
}
