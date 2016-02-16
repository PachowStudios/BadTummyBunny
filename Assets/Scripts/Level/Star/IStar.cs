using PachowStudios.BadTummyBunny.UserData;

namespace PachowStudios.BadTummyBunny
{
  public interface IStar
  {
    string Id { get; }
    string Name { get; }

    StarRequirement Requirement { get; }
    CompletionState CompletionState { get; }
  }
}