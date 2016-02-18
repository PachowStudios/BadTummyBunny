namespace PachowStudios.BadTummyBunny
{
  public interface IStarController
  {
    IStar Star { get; }
    CompletionState CompletionState { get; } 
  }
}