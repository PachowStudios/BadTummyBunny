public interface IEventAggregator
{
  bool HandlerExistsFor<TMessage>()
    where TMessage : IMessage;

  void Subscribe<THandler>(THandler subscriber)
    where THandler : IHandles;

  void Unsubscribe<THandler>(THandler subscriber)
    where THandler : IHandles;

  void Publish<TMessage>(TMessage message)
    where TMessage : IMessage;
}