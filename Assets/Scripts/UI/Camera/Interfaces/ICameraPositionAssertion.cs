using UnityEngine;

public interface ICameraPositionAssertion
{
	Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAvgVelocity);
}
