using UnityEngine;

[AddComponentMenu("UI/Camera/Effectors/Fart Aim Lean")]
public class FartAimLean : MonoBehaviour, ICameraEffector
{
	public float effectorWeight = 5f;
	public float leanDistance = 3f;
	public float minimumPower = 0.2f;
	public AnimationCurve effectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private bool isEnabled = false;

	private PlayerControl Player
	{ get { return PlayerControl.Instance; } }

	private void Update()
	{
		if (Player.IsFartCharging && !isEnabled)
			Activate();
		else if (!Player.IsFartCharging && isEnabled)
			Deactivate();
	}

	public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
	{
		var targetPosition = Player.transform.position;

		targetPosition += leanDistance * Player.FartDirection.ToVector3();

		return targetPosition;
	}

	public float GetEffectorWeight()
	{
		var startingPower = Player.FartPower;

		if (startingPower < minimumPower)
			return 0f;

		var adjustedPower = Extensions.ConvertRange(startingPower, 0f, 1f, minimumPower, 1f);

		return effectorWeight * effectorFalloff.Evaluate(adjustedPower);
	}

	private void Activate()
	{
		isEnabled = true;
		CameraController.Instance.AddCameraEffector(this);
	}

	private void Deactivate()
	{
		isEnabled = false;
		CameraController.Instance.RemoveCameraEffector(this);
	}
}