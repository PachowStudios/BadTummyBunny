public interface IFiniteState<T> where T : class
{
	IFiniteState<T> Initialize(IFiniteStateMachine<T> stateMachine, T context);
	void OnInitialized();
	void Begin();
	void Reason();
	void Update(float deltaTime);
	void End();
}