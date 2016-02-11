using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseStar<TConfig> : IStar
    where TConfig : StarSettings
  {
    protected abstract TConfig Config { get; set; }

    [Inject] protected IEventAggregator EventAggregator { get; private set; }

    public string Id => Config.Id;
    public string Name => Config.Name;
    public StarRequirement Requirement => Config.Requirement;

    public bool Completed { get; private set; }

    protected virtual void OnCompleted() { }

    protected virtual void Complete()
    {
      if (Completed)
        return;

      Completed = true;
      EventAggregator.Publish(new StarCompletedMessage(this));
      OnCompleted();
    }
  }

  public class StarCompletedMessage : IMessage
  {
    public IStar Star { get; }

    public StarCompletedMessage(IStar star)
    {
      Star = star;
    }
  }
}