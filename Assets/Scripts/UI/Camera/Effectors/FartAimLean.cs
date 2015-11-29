using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/UI/Camera/Effectors/Fart Aim Lean")]
  public sealed class FartAimLean : MonoBehaviour, ICameraEffector
  {
    [SerializeField] private float effectorWeight = 5f;
    [SerializeField] private float leanDistance = 3f;
    [SerializeField] private float minimumPower = 0.2f;
    [SerializeField] private AnimationCurve effectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Inject]
    private CameraController CameraController { get; set; }

    [Inject(Tags.Player)]
    private IFartInfoProvider FartInfo { get; set; }

    [Inject(Tags.Player)]
    private IMovable PlayerMovement { get; set; }

    public bool IsEnabled { get; private set; }

    private void Update()
    {
      if (FartInfo.IsFartCharging && !IsEnabled)
        Activate();
      else if (!FartInfo.IsFartCharging && IsEnabled)
        Deactivate();
    }

    public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
    {
      var targetPosition = PlayerMovement.Position;

      targetPosition += this.leanDistance * FartInfo.FartDirection.ToVector3();

      return targetPosition;
    }

    public float GetEffectorWeight()
    {
      var startingPower = FartInfo.FartPower;

      if (startingPower < this.minimumPower)
        return 0f;

      var adjustedPower = Extensions.ConvertRange(startingPower, 0f, 1f, this.minimumPower, 1f);

      return this.effectorWeight * this.effectorFalloff.Evaluate(adjustedPower);
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
