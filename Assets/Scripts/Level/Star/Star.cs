using PachowStudios.BadTummyBunny.UserData;

namespace PachowStudios.BadTummyBunny
{
  public class Star : IStar
  {
    public string Id => Config.Id;
    public string Name => Config.Name;
    public StarRequirement Requirement => Config.Requirement;

    public bool IsCompleted
    {
      get { return Progress.IsCompleted; }
      set { Progress.IsCompleted = value; }
    }

    public BaseStarSettings Config { get; }

    private StarProgress Progress { get; }

    public Star(BaseStarSettings config, StarProgress progress)
    {
      Config = config;
      Progress = progress;
    }
  }
}