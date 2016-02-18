using PachowStudios.Assertions;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseStarController : IStarController
  {
    private CompletionState completionState = CompletionState.InProgress;

    [Inject] public IStar Star { get; private set; }

    public CompletionState CompletionState
    {
      get { return Star.IsCompleted ? CompletionState.Completed : this.completionState; }
      private set
      {
        this.completionState = value;

        if (CompletionState == CompletionState.Completed)
          Star.IsCompleted = true;
      }
    }

    [Inject] protected IEventAggregator EventAggregator { get; private set; }

    protected virtual void OnCompleted() { }

    protected virtual void OnFailed() { }

    protected void Complete()
    {
      CompletionState.Should().NotBe(CompletionState.Failed, "because failed stars cannot be completed");

      if (CompletionState != CompletionState.InProgress)
        return;

      CompletionState = CompletionState.Completed;
      EventAggregator.Publish(new StarCompletedMessage(Star));
      OnCompleted();
    }

    protected void Fail()
    {
      CompletionState.Should().NotBe(CompletionState.Completed, "because completed stars cannot be failed");

      if (CompletionState != CompletionState.InProgress)
        return;

      CompletionState = CompletionState.Failed;
      EventAggregator.Publish(new StarFailedMessage(Star));
      OnFailed();
    }
  }
}