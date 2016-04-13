using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public sealed class FartAimLean : ICameraEffector, ITickable
  {
    private bool IsEnabled { get; set; }

    private FartAimLeanSettings Config { get; }
    private IView View { get; }
    private IFartInfoProvider FartInfo { get; }
    private CameraController CameraController { get; }

    public FartAimLean(FartAimLeanSettings config, IView view, IFartInfoProvider fartInfo, CameraController cameraController)
    {
      Config = config;
      View = view;
      FartInfo = fartInfo;
      CameraController = cameraController;
    }

    public void Tick()
    {
      if (FartInfo.IsFartAiming && !IsEnabled)
        Activate();
      else if (!FartInfo.IsFartAiming && IsEnabled)
        Deactivate();
    }

    public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
    {
      var targetPosition = View.Position;

      targetPosition += Config.LeanDistance * FartInfo.FartDirection.ToVector3();

      return targetPosition;
    }

    public float GetEffectorWeight()
    {
      if (FartInfo.FartPower < Config.MinimumPower)
        return 0f;

      return Config.EffectorWeight * Config.EffectorFalloff.Evaluate(
        MathHelper.LerpRange(
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
