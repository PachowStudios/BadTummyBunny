using UnityEngine;

[AddComponentMenu("UI/Camera/Effectors/Fart Aim Lean")]
public sealed class FartAimLean : MonoBehaviour, ICameraEffector
{
	[SerializeField]
	private float effectorWeight = 5f;
	[SerializeField]
	private float leanDistance = 3f;
	[SerializeField]
	private float minimumPower = 0.2f;
	[SerializeField]
	private AnimationCurve effectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public bool IsEnabled { get; private set; } = false;

	private IFartStatusProvider FartStatusProvider => Player.Instance.FartStatusProvider;

	private void Update()
	{
		if (Player.Instance.FartStatusProvider.IsFartCharging && !IsEnabled)
			Activate();
		else if (!FartStatusProvider.IsFartCharging && IsEnabled)
			Deactivate();
	}

	public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
	{
		var targetPosition = Player.Instance.Movement.Position;

		targetPosition += leanDistance * FartStatusProvider.FartDirection.ToVector3();

		return targetPosition;
	}

	public float GetEffectorWeight()
	{
		var startingPower = FartStatusProvider.FartPower;

		if (startingPower < minimumPower)
			return 0f;

		var adjustedPower = Extensions.ConvertRange(startingPower, 0f, 1f, minimumPower, 1f);

		return effectorWeight * effectorFalloff.Evaluate(adjustedPower);
	}

	private void Activate()
	{
		IsEnabled = true;
		CameraController.Instance.AddCameraEffector(this);
	}

	private void Deactivate()
	{
		IsEnabled = false;
		CameraController.Instance.RemoveCameraEffector(this);
	}
}