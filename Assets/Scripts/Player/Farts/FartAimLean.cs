using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class FartAimLean : ICameraEffector, ITickable
  {
    [InstallerSettings]
    public class Settings
    {
      public float EffectorWeight = 5f;
      public float LeanDistance = 3f;
      public float MinimumPower = 0.2f;
      public AnimationCurve EffectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    }

    [InjectLocal] private Settings Config { get; set; }
    [InjectLocal] private IFartInfoProvider FartInfo { get; set; }
    [InjectLocal] private IMovable Movement { get; set; }

    [Inject] private CameraController CameraController { get; set; }

    public bool IsEnabled { get; private set; }

    public void Tick()
    {
      if (FartInfo.IsFartCharging && !IsEnabled)
        Activate();
      else if (!FartInfo.IsFartCharging && IsEnabled)
        Deactivate();
    }

    public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
    {
      var targetPosition = Movement.Position;

      targetPosition += Config.LeanDistance * FartInfo.FartDirection.ToVector3();

      return targetPosition;
    }

    public float GetEffectorWeight()
    {
      if (FartInfo.FartPower < Config.MinimumPower)
        return 0f;

      return Config.EffectorWeight * Config.EffectorFalloff.Evaluate(
        MathHelper.ConvertRange(
          FartInfo.FartPower,
          0f, 1f,
          Config.MinimumPower, 1f));
    }

    private void Activate()
    {
      IsEnabled = true;
      CameraController.AddCameraEffector(this);
    }

    private void Deactivate()
    {
      IsEnabled = false;
      CameraController.RemoveCameraEffector(this);
    }
  }
}
