using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IFart : IAttachable<PlayerView>
  {
    string Name { get; }
    FartType Type { get; }

    bool IsFarting { get; }
    bool ShowTrajectory { get; set; }

    void StartFart(float power);
    void StopFart();
    float CalculateSpeed(float power);
    void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition);
  }
}
