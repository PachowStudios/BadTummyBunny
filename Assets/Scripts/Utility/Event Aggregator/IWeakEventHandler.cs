public partial class EventAggregator
{
  private interface IWeakEventHandler
  {
    bool IsAlive { get; }

    bool Handle<TMessage>(TMessage message);
    bool Handles<TMessage>();
    bool ReferenceEquals(object instance);
  }
}