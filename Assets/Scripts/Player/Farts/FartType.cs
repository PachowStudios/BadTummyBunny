namespace PachowStudios.BadTummyBunny
{
  public enum FartType
  {
    [TypeMapping(typeof(Fart))]
    Basic,
    [TypeMapping(typeof(StatusEffectFart))]
    Flaming
  }
}