namespace PachowStudios.BadTummyBunny
{
  public interface ICharacter
  {
    IView View { get; }
    IMovable Movement { get; }
    IHasHealth Health { get; }
  }
}