using JetBrains.Annotations;

namespace PachowStudios.BadTummyBunny
{
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public enum StarRequirement
  {
    [TypeMapping(typeof(CollectCoinsStar))]
    CollectCoins,
    KillEnemies,
    CompletionTime
  }
}