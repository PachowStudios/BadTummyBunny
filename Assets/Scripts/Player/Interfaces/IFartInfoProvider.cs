using UnityEngine;

public interface IFartInfoProvider
{
  bool IsFartCharging { get; }
  float FartPower { get; }
  Vector2 FartDirection { get; }
}
