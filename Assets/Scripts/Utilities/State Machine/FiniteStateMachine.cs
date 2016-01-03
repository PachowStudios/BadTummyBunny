using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

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

    public FiniteStateMachine(T context)
    {
      this.context = context;
    }

    public FiniteStateMachine<T> Add<TState>()
      where TState : FiniteState<T>
    {
      this.states[typeof(TState)] = ReflectionHelper.Create<TState>(this, this.context);

      if (CurrentState == null)
      {
        CurrentState = this.states[typeof(TState)];
        CurrentState.Begin();
      }

      return this;
    }

    public TState GoTo<TState>()
      where TState : FiniteState<T>
    {
      var matchedState = CurrentState as TState;

      if (matchedState != null)
        return matchedState;

      CurrentState?.End();

      Assert.IsTrue(this.states.ContainsKey(typeof(TState)), $"{GetType()} : state {typeof(TState)} doesn't exist!");

      PreviousState = CurrentState;
      CurrentState = this.states[typeof(TState)];
      CurrentState.Begin();
      ElapsedTimeInState = 0f;
      StateChanged?.Invoke();

      return (TState)CurrentState;
    }

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
