public abstract class FiniteState<T> : IFiniteState<T>
  where T : class
{
  protected IFiniteStateMachine<T> StateMachine { get; private set; }
  protected T Context { get; private set; }

  public IFiniteState<T> Initialize(IFiniteStateMachine<T> stateMachine, T context)
  {
    StateMachine = stateMachine;
    Context = context;
    OnInitialized();

    return this;
  }

  public virtual void OnInitialized() { }
  public virtual void Begin() { }
  public virtual void Reason() { }
  public virtual void Update(float deltaTime) { }
  public virtual void End() { }
}
