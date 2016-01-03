using JetBrains.Annotations;

namespace PachowStudios
{
  public interface IEventAggregator
  {
    void Subscribe<THandler>([NotNull] THandler subscriber)
      where THandler : IHandles;

    void Unsubscribe<THandler>([NotNull] THandler subscriber)
      where THandler : IHandles;

    void Publish<TMessage>([NotNull] TMessage message)
      where TMessage : IMessage;

    [Pure]
    bool HandlerExistsFor<TMessage>()
      where TMessage : IMessage;
  }
}