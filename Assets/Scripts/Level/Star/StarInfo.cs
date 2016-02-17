using PachowStudios.BadTummyBunny.UserData;

namespace PachowStudios.BadTummyBunny
{
  public class StarInfo : IStar
  {
    public string Id => Config.Id;
    public string Name => Config.Name;
    public StarRequirement Requirement => Config.Requirement;
    public CompletionState CompletionState => Progress.IsCompleted ? CompletionState.Completed : CompletionState.InProgress;

    private BaseStarSettings Config { get; }
    private StarProgress Progress { get; }

    public StarInfo(BaseStarSettings config, StarProgress progress)
    {
      Config = config;
      Progress = progress;
    }
  }
}