public interface IHandles { }

public interface IHandles<in TMessage> : IHandles
{
  void Handle(TMessage message);
}

public interface IHandles<in TMessage1, in TMessage2> : IHandles<TMessage1>
{
  void Handle(TMessage2 message);
}

public interface IHandles<in TMessage1, in TMessage2, in TMessage3> : IHandles<TMessage1, TMessage2>
{
  void Handle(TMessage3 message);
}

public interface IHandles<in TMessage1, in TMessage2, in TMessage3, in TMessage4> : IHandles<TMessage1, TMessage2, TMessage3>
{
  void Handle(TMessage4 message);
}

public interface IHandles<in TMessage1, in TMessage2, in TMessage3, in TMessage4, in TMessage5> : IHandles<TMessage1, TMessage2, TMessage3, TMessage4>
{
  void Handle(TMessage5 message);
}