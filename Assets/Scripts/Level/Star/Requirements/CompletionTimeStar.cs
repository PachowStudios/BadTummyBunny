using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class CompletionTimeStar : BaseStar<CompletionTimeStarSettings>, ITickable,
    IHandles<LevelCompletedMessage>
  {
    [Inject] protected override CompletionTimeStarSettings Config { get; set; }

    private float elapsedTime;

    private float ElapsedTime
    {
      get { return this.elapsedTime; }
      set
      {
        this.elapsedTime = value;

        if (this.elapsedTime > Config.TimeLimit)
          Fail();
      }
    }

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    public void Tick()
      => ElapsedTime += Time.deltaTime;

    protected override void OnCompleted()
      => EventAggregator.Unsubscribe(this);

    protected override void OnFailed()
      => EventAggregator.Unsubscribe(this);

    public void Handle(LevelCompletedMessage message)
    {
      if (ElapsedTime <= Config.TimeLimit)
        Complete();
    }
  }
}