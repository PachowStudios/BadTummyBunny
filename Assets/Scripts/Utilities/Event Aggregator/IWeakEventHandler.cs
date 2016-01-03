using JetBrains.Annotations;

namespace PachowStudios
{
  public partial class EventAggregator
  {
    private interface IWeakEventHandler
    {
      bool IsAlive { get; }

      bool Handle<TMessage>([NotNull] TMessage message)
        where TMessage : IMessage;

      [Pure]
      bool Handles<TMessage>()
        where TMessage : IMessage;

      [Pure]
      bool ReferenceEquals(object instance);
    }
  }
}