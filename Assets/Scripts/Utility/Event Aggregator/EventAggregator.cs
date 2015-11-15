using System.Collections.Generic;
using System.Linq;

public partial class EventAggregator : IEventAggregator
{
  private readonly List<IWeakEventHandler> handlers = new List<IWeakEventHandler>();

  public bool HandlerExistsFor<TMessage>()
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
    => this.handlers
         .Where(h => !h.Handle(message))
         .ForEach(h => this.handlers.Remove(h));
}