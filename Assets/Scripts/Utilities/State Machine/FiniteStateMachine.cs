using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PachowStudios.Assertions;

namespace PachowStudios
{
  public sealed class FiniteStateMachine<T>
    where T : class
  {
    public event Action StateChanged;

    public FiniteState<T> CurrentState { get; private set; }
    public FiniteState<T> PreviousState { get; private set; }
    public float ElapsedTimeInState { get; private set; }

    private readonly Dictionary<Type, FiniteState<T>> states = new Dictionary<Type, FiniteState<T>>();
    private readonly T context;

    public FiniteStateMachine([NotNull] T context)
    {
      this.context = context;
    }

    [NotNull]
    public FiniteStateMachine<T> Add<TState>()
      where TState : FiniteState<T>
    {
      this.states[typeof(TState)] = ReflectionHelper.Create<TState>(this, this.context);

      if (CurrentState == null)
      {
        CurrentState = this.states[typeof(TState)];
        CurrentState.Enter();
      }

      return this;
    }

    public void GoTo<TState>(bool immediate = false)
      where TState : FiniteState<T>
    {
      if (CurrentState is TState)
        return;

      if (!immediate)
        CurrentState.Leave();

      this.states.Should().ContainKey(typeof(TState), $"because state {typeof(TState)} must exist.");

      PreviousState = CurrentState;
      CurrentState = this.states[typeof(TState)];
      CurrentState.Enter();
      ElapsedTimeInState = 0f;
      StateChanged?.Invoke();
    }

    [Pure]
    public bool CameFrom<TState>()
      where TState : FiniteState<T>
      => PreviousState is TState;

    public void Tick(float deltaTime)
    {
      ElapsedTimeInState += deltaTime;
      CurrentState.Reason();
      CurrentState.Tick(deltaTime);
    }
  }
}
