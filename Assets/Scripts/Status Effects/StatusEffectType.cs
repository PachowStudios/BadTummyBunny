using JetBrains.Annotations;

namespace PachowStudios.BadTummyBunny
{
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public enum StatusEffectType
  {
    [TypeMapping(typeof(BurningStatusEffect))]
    Burning
  }
}