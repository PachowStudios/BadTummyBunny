using System.Diagnostics;
using UnityEngine;

public class MapExtentsFinalizer : MonoBehaviour, ICameraFinalizer
{
	public bool snapToBottom;
	public bool snapToTop;
	public bool snapToRight;
	public bool snapToLeft;

	public float bottomConstraint;
	public float topConstraint;
	public float rightConstraint;
	public float leftConstraint;

	public bool IsEnabled
	{ get { return enabled; } }

	public int GetFinalizerPriority
	{ get { return 0; } }

	public bool ShouldSkipSmoothingThisFrame
	{ get { return false; } }

	// this is only here so that we get the "Enabled" checkbox in the Inspector
	[Conditional("UNITY_EDITOR")]
	private void Update() { }

	#region ICameraFinalizer
	public Vector3 GetFinalCameraPosition(Bounds targetBounds, Vector3 currentCameraPosition, Vector3 desiredCameraPosition)
	{
		// orthographicSize is 0.5 * height. aspect is width / height. that makes this calculation equal 0.5 * width
		var orthoSize = CameraController.Instance.camera.orthographicSize;
		var orthoHalfWidth = orthoSize * CameraController.Instance.camera.aspect;

		// clamp the camera position to the maps bounds
		// left
		if (snapToLeft && desiredCameraPosition.x - orthoHalfWidth < leftConstraint)
			desiredCameraPosition.x = leftConstraint + orthoHalfWidth;

		// right
		if (snapToRight && desiredCameraPosition.x + orthoHalfWidth > rightConstraint)
			desiredCameraPosition.x = rightConstraint - orthoHalfWidth;

		// top
		if (snapToTop && desiredCameraPosition.y + orthoSize > topConstraint)
			desiredCameraPosition.y = topConstraint - orthoSize;

		// bottom
		if (snapToBottom && desiredCameraPosition.y - orthoSize < bottomConstraint)
			desiredCameraPosition.y = bottomConstraint + orthoSize;

		return desiredCameraPosition;
	}

	#if UNITY_EDITOR
	public void onDrawGizmos(Vector3 basePosition)
	{
		const int FakeInfinity = 10000;

		Gizmos.color = Color.red;

		if (snapToBottom) Gizmos.DrawLine(new Vector2(-FakeInfinity, bottomConstraint), 
																			new Vector2(FakeInfinity, bottomConstraint));
		if (snapToTop) Gizmos.DrawLine(new Vector2(-FakeInfinity, topConstraint), 
																		new Vector2(FakeInfinity, topConstraint));
		if (snapToRight) Gizmos.DrawLine(new Vector2(rightConstraint, -FakeInfinity),
																			new Vector2(rightConstraint, FakeInfinity));
		if (snapToLeft) Gizmos.DrawLine(new Vector2(leftConstraint, -FakeInfinity),
																		new Vector2(leftConstraint, FakeInfinity));
	}
	#endif
	#endregion
}
