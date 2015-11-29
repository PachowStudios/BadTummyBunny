using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public sealed class FiniteStateMachine<T> : IFiniteStateMachine<T>
  where T : class
{
  public event Action StateChanged;

  public IFiniteState<T> CurrentState { get; private set; }
  public IFiniteState<T> PreviousState { get; private set; }
  public float ElapsedTimeInState { get; private set; }

  private Dictionary<Type, IFiniteState<T>> states = new Dictionary<Type, IFiniteState<T>>();
  private T context;

  public FiniteStateMachine(T context)
  {
    this.context = context;
  }

  public IFiniteStateMachine<T> AddState<TState>()
    where TState : IFiniteState<T>, new()
  {
    this.states[typeof(TState)] = new TState().Initialize(this, this.context);

    if (CurrentState == null)
    {
      CurrentState = this.states[typeof(TState)];
      CurrentState.Begin();
    }

    return this;
  }

  public TState GoToState<TState>()
    where TState : IFiniteState<T>
  {
    if (CurrentState is TState)
      return (TState)CurrentState;

    CurrentState?.End();

    Assert.IsTrue(this.states.ContainsKey(typeof(TState)), $"{GetType()} : state {typeof(TState)} doesn't exist!");

    PreviousState = CurrentState;
    CurrentState = this.states[typeof(TState)];
    CurrentState.Begin();
    ElapsedTimeInState = 0f;
    StateChanged?.Invoke();

    return (TState)CurrentState;
  }

  public bool CameFromState<TState>()
    where TState : IFiniteState<T>
    => PreviousState is TState;

  public void Update(float deltaTime)
  {
    ElapsedTimeInState += deltaTime;
    CurrentState.Reason();
    CurrentState.Update(deltaTime);
  }
}
