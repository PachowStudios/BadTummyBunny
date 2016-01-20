namespace PachowStudios.BadTummyBunny
{
  public enum FartType
  {
    [TypeMapping(typeof(BasicFart))]
    Basic,
    [TypeMapping(typeof(StatusEffectFart))]
    Flaming
  }
}