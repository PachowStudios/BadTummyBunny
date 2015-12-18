using System;

namespace PachowStudios
{
  public interface IFiniteStateMachine<T>
    where T : class
  {
    event Action StateChanged;

    IFiniteState<T> CurrentState { get; }
    IFiniteState<T> PreviousState { get; }
    float ElapsedTimeInState { get; }

    IFiniteStateMachine<T> Add<TState>() where TState : IFiniteState<T>;
    TState GoTo<TState>() where TState : IFiniteState<T>;
    bool CameFrom<TState>() where TState : IFiniteState<T>;
    void Tick(float deltaTime);
  }
}