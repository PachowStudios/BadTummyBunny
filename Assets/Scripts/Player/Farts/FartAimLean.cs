using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class FartAimLean : ICameraEffector, ITickable
  {
    [InjectLocal] private FartAimLeanSettings Config { get; set; }
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
