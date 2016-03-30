using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StarCompletionHandler : IInitializable,
    IHandles<StarCompletedMessage>,
    IHandles<StarFailedMessage>
  {
    [Inject] private LevelSettings Config { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; }

    [InjectOptional] private IEnumerable<IStarController> StarControllers { get; set; }

    public void Initialize()
      => EventAggregator.Subscribe(this);

    public void Handle(StarCompletedMessage message)
      => Debug.Log($"{message.Star.Name} completed");

    public void Handle(StarFailedMessage message)
      => Debug.Log($"{message.Star.Name} failed");
  }
}