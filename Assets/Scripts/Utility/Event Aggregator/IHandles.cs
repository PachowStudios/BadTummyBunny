public interface IHandles { }

public interface IHandles<in TMessage> : IHandles
  where TMessage : IMessage
{
  void Handle(TMessage message);
}