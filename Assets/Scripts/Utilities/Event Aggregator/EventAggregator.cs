using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;

namespace PachowStudios
{
  public partial class EventAggregator : IEventAggregator
  {
    private readonly List<IWeakEventHandler> handlers = new List<IWeakEventHandler>();

    public bool HandlerExistsFor<TMessage>()
      where TMessage : IMessage
      => this.handlers.Any(h => h.Handles<TMessage>() && h.IsAlive);

    public void Subscribe<THandler>(THandler subscriber)
      where THandler : IHandles
    {
      if (this.handlers.None(h => h.ReferenceEquals(subscriber)))
        this.handlers.Add(new WeakEventHandler<THandler>(subscriber));
    }

    public void Unsubscribe<THandler>(THandler subscriber)
      where THandler : IHandles
      => this.handlers.RemoveAll(h => h.ReferenceEquals(subscriber));

    public void Publish<TMessage>(TMessage message)
      where TMessage : IMessage
      => this.handlers.RemoveAll(h => !h.Handle(message));
  }
}