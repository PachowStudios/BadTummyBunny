using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IFart : IAttachable<PlayerView>
  {
    string Name { get; }
    FartType Type { get; }

    bool IsFarting { get; }

    void StartFart(float power, Vector2 direction);
    void StopFart();
    float CalculateSpeed(float power);
    void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition);
    void ClearTrajectory();
  }
}
