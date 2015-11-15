public partial class EventAggregator
{
  private interface IWeakEventHandler
  {
    bool IsAlive { get; }

    bool Handle<TMessage>(TMessage message)
      where TMessage : IMessage;

    bool Handles<TMessage>()
      where TMessage : IMessage;

    bool ReferenceEquals(object instance);
  }
}