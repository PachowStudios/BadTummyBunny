namespace PachowStudios.BadTummyBunny
{
  public interface IStatusEffectView
  {
    void Attach(IStatusEffectable affectedCharacter);
    void Detach();
  }
}