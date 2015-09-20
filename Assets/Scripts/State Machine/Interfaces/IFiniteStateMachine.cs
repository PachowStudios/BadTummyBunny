using System;

public interface IFiniteStateMachine<T> where T : class
{
	event Action StateChanged;

	IFiniteState<T> CurrentState { get; }
	IFiniteState<T> PreviousState { get; }
	float ElapsedTimeInState { get; }

	IFiniteStateMachine<T> AddState<TState>() where TState : IFiniteState<T>, new();
	TState GoToState<TState>() where TState : IFiniteState<T>;
	bool CameFromState<TState>() where TState : IFiniteState<T>;
	void Update(float deltaTime);
}