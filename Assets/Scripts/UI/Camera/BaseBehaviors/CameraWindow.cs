using System.Diagnostics;
using UnityEngine;

public class CameraWindow : MonoBehaviour, ICameraBaseBehavior
{
	[Range(0f, 20f)]
	public float width = 3f;
	[Range(0f, 20f)]
	public float height = 3f;
	[BitMask]
	public CameraAxis axis;

	public bool IsEnabled
	{ get { return enabled; } }

	private bool AffectHorizontal
	{ get { return (axis & CameraAxis.Horizontal) == CameraAxis.Horizontal; } }

	private bool AffectVertical
	{ get { return (axis & CameraAxis.Vertical) == CameraAxis.Vertical; } }

	private bool AffectBothAxis
	{ get { return AffectHorizontal && AffectVertical; } }

	[Conditional("UNITY_EDITOR")]
	private void Update() { }

	public Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity)
	{
		var desiredOffset = Vector3.zero;
		var bounds = new Bounds(basePosition, new Vector3(width, height, 5f));

		if (!bounds.Contains(targetBounds.max) || !bounds.Contains(targetBounds.min))
		{
			if (AffectHorizontal)
			{
				if (bounds.min.x > targetBounds.min.x)
					desiredOffset.x = targetBounds.min.x - bounds.min.x;
				else if (bounds.max.x < targetBounds.max.x)
					desiredOffset.x = targetBounds.max.x - bounds.max.x;
			}

			if (AffectVertical)
			{
				if (bounds.min.y > targetBounds.min.y)
					desiredOffset.y = targetBounds.min.y - bounds.min.y;
				else if (bounds.max.y < targetBounds.max.y)
					desiredOffset.y = targetBounds.max.y - bounds.max.y;
			}
		}

		return desiredOffset;
	}

	#if UNITY_EDITOR
	public void onDrawGizmos(Vector3 basePosition)
	{
		Gizmos.color = new Color(1f, 0f, 0.6f);

		var bounds = new Bounds(basePosition, new Vector3(width, height));
		var lineWidth = Camera.main.orthographicSize;

		if (AffectVertical && !AffectBothAxis)
			bounds.Expand(new Vector3(lineWidth - bounds.size.x, 0f));

		if (AffectHorizontal && !AffectBothAxis)
			bounds.Expand(new Vector3(0f, lineWidth - bounds.size.y));

		if (AffectVertical || AffectBothAxis)
		{
			Gizmos.DrawLine(bounds.min, bounds.min + new Vector3(bounds.size.x, 0f));
			Gizmos.DrawLine(bounds.max, bounds.max - new Vector3(bounds.size.x, 0f));
		}

		if (AffectHorizontal || AffectBothAxis)
		{
			Gizmos.DrawLine(bounds.min, bounds.min + new Vector3(0f, bounds.size.y));
			Gizmos.DrawLine(bounds.max, bounds.max - new Vector3(0f, bounds.size.y));
		}
	}
	#endif
}