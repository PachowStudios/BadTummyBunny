using PachowStudios.Assertions;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class LevelCompletionHandler : ILevelCompletionHandler
  {
    public bool IsCompleted { get; private set; }

    [Inject] private Scene CurrentScene { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; }

    public void CompleteLevel()
    {
      IsCompleted.Should().BeFalse("because the level can only be completed once");

      IsCompleted = true;
      EventAggregator.Publish(new LevelCompletedMessage(CurrentScene));
    }
  }
}