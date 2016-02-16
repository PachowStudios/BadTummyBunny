using PachowStudios.Assertions;
using PachowStudios.BadTummyBunny.UserData;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseStar<TConfig> : IStar
    where TConfig : BaseStarSettings
  {
    private CompletionState completionState = CompletionState.InProgress;

    public string Id => Config.Id;
    public string Name => Config.Name;
    public StarRequirement Requirement => Config.Requirement;

    public CompletionState CompletionState
    {
      get { return this.completionState; }
      private set
      {
        this.completionState = value;

        if (CompletionState == CompletionState.Completed)
          Progress.IsCompleted = true;
      }
    }

    protected abstract TConfig Config { get; set; }

    [Inject] protected StarProgress Progress { get; private set; }
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