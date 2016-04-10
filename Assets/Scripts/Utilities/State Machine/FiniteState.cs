using JetBrains.Annotations;

namespace PachowStudios
{
  [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
  public abstract class FiniteState<T>
    where T : class
  {
    [NotNull] protected FiniteStateMachine<T> StateMachine { get; }
    [NotNull] protected T Context { get; }

    protected FiniteState([NotNull] FiniteStateMachine<T> stateMachine, [NotNull] T context)
    {
      StateMachine = stateMachine;
      Context = context;
    }

    public virtual void Enter() { }
    public virtual void Reason() { }
    public virtual void Tick(float deltaTime) { }
    public virtual void Leave() { }
  }
}