using UnityEngine;

public interface IFartStatusProvider
{
	bool IsFartCharging { get; }
	float FartPower { get; }
	Vector2 FartDirection { get; }
}