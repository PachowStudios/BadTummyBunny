using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public interface IFart : IAttachable<PlayerView>, ITickable
  {
    string Name { get; }
    FartType Type { get; }

    bool IsFarting { get; }
    bool IsSecondaryFarting { get; }
    bool CanFart { get; }
    float TimeFarting { get; }
    bool ShowTrajectory { get; set; }

    void StartFarting(float power);
    void SecondaryFart(float power);
    void StopFarting();
    float CalculateSpeed(float power);
    void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition);
  }
}
