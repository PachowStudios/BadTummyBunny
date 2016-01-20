using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace PachowStudios
{
  public partial class EventAggregator
  {
    private class WeakEventHandler<THandler> : IWeakEventHandler
      where THandler : IHandles
    {
      private WeakReference Target { get; }
      private Dictionary<Type, MethodInfo> HandlerMethods { get; } = new Dictionary<Type, MethodInfo>();

      public bool IsAlive => Target.IsAlive;

      public WeakEventHandler([NotNull] THandler handler)
      {
        Target = new WeakReference(handler);

        foreach (var messageType in
          typeof(THandler).GetInterfaces()
            .Where(i => i.IsAssignableFrom<IHandles<IMessage>>() && i.IsGenericType)
            .Select(i => i.GetGenericArguments().First()))
          HandlerMethods[messageType] =
            typeof(THandler).GetMethod(
              nameof(IHandles<IMessage>.Handle),
              new[] { messageType });
      }

      public bool Handle<TMessage>(TMessage message)
        where TMessage : IMessage
      {
        if (!IsAlive)
          return false;

        foreach (var handler in HandlerMethods.Where(h => h.Key.IsAssignableFrom<TMessage>()))
          handler.Value.Invoke(Target.Target, new object[] { message });

        return true;
      }

      [Pure]
      public bool Handles<TMessage>()
        where TMessage : IMessage
        => HandlerMethods.Any(h => h.Key.IsAssignableFrom<TMessage>());

      [Pure]
      public bool ReferenceEquals(object instance)
        => ReferenceEquals(Target.Target, instance);
    }
  }
}