using JetBrains.Annotations;

namespace PachowStudios.BadTummyBunny
{
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public enum FartType
  {
    [TypeMapping(typeof(BasicFart))]
    Basic,
    [TypeMapping(typeof(StatusEffectFart))]
    Flaming
  }
}