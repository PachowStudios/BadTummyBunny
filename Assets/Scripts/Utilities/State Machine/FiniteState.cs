namespace PachowStudios
{
  public abstract class FiniteState<T> : IFiniteState<T>
    where T : class
  {
    protected IFiniteStateMachine<T> StateMachine { get; }
    protected T Context { get; }

    protected FiniteState(IFiniteStateMachine<T> stateMachine, T context)
    {
      StateMachine = stateMachine;
      Context = context;
    }

    public virtual void Begin() { }
    public virtual void Reason() { }
    public virtual void Tick(float deltaTime) { }
    public virtual void End() { }
  }

}