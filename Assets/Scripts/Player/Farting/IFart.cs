using UnityEngine;

public interface IFart
{
	bool IsFarting { get; }

	void StartFart(float power, Vector2 direction);
	void StopFart();
	float CalculateSpeed(float power);
	void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition);
	void ClearTrajectory();
}