using PachowStudios.Assertions;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseStar<TConfig> : IStar
    where TConfig : BaseStarSettings
  {
    public string Id => Config.Id;
    public string Name => Config.Name;
    public StarRequirement Requirement => Config.Requirement;

    public CompletionState CompletionState { get; private set; } = CompletionState.InProgress;

    protected abstract TConfig Config { get; set; }

    [Inject] protected IEventAggregator EventAggregator { get; private set; }

    protected virtual void OnCompleted() { }

    protected virtual void OnFailed() { }

    protected void Complete()
    {
      CompletionState.Should().NotBe(CompletionState.Failed, "because failed stars cannot be completed");

      if (CompletionState != CompletionState.InProgress)
        return;

      CompletionState = CompletionState.Completed;
      EventAggregator.Publish(new StarCompletedMessage(this));
      OnCompleted();
    }

    protected void Fail()
    {
      CompletionState.Should().NotBe(CompletionState.Completed, "because completed stars cannot be failed");

      if (CompletionState != CompletionState.InProgress)
        return;

      CompletionState = CompletionState.Failed;
      EventAggregator.Publish(new StarFailedMessage(this));
      OnFailed();
    }
  }
}