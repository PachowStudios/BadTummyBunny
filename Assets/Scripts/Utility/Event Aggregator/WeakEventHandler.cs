using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class EventAggregator
{
  private class WeakEventHandler<THandler> : IWeakEventHandler
    where THandler : IHandles
  {
    private readonly WeakReference reference;
    private readonly Dictionary<Type, MethodInfo> handlers = new Dictionary<Type, MethodInfo>();

    public bool IsAlive => this.reference.IsAlive;

    public WeakEventHandler(THandler handler)
    {
      this.reference = new WeakReference(handler);

      foreach (var messageType in typeof(THandler).GetGenericArguments())
        this.handlers[messageType] = typeof(THandler).GetMethod(nameof(IHandles<object>.Handle), new[] { messageType });
    }

    public bool Handle<TMessage>(TMessage message)
    {
      if (!IsAlive)
        return false;

      foreach (var handler in this.handlers.Where(h => h.Key.IsAssignableFrom(typeof(TMessage))))
        handler.Value.Invoke(this.reference.Target, new object[] { message });

      return true;
    }

    public bool Handles<TMessage>()
      => this.handlers.Any(h => h.Key.IsAssignableFrom(typeof(TMessage)));

    public bool ReferenceEquals(object instance)
      => ReferenceEquals(this.reference.Target, instance);
  }
}