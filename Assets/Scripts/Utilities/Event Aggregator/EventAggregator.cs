using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using JetBrains.Annotations;

namespace PachowStudios
{
  public partial class EventAggregator : IEventAggregator
  {
    private List<IWeakEventHandler> Handlers { get; } = new List<IWeakEventHandler>();

    public void Subscribe<THandler>(THandler subscriber)
      where THandler : IHandles
    {
      if (Handlers.None(h => h.ReferenceEquals(subscriber)))
        Handlers.Add(new WeakEventHandler<THandler>(subscriber));
    }

    public void Unsubscribe<THandler>(THandler subscriber)
      where THandler : IHandles
      => Handlers.RemoveSingle(h => h.ReferenceEquals(subscriber));

    public void Publish<TMessage>(TMessage message)
      where TMessage : IMessage
      => Handlers.RemoveAll(h => !h.Handle(message));

    [Pure]
    public bool HandlerExistsFor<TMessage>()
      where TMessage : IMessage
      => Handlers.Any(h => h.Handles<TMessage>() && h.IsAlive);
  }
}