using UnityEngine;

public interface ICameraBaseBehavior : ICameraPositionAssertion
{
	bool IsEnabled { get; }

	#if UNITY_EDITOR
	// useful for while we are in the editor to provide a UI
	void onDrawGizmos(Vector3 basePosition);
	#endif
}
