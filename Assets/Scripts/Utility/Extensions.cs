﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
	#region Float
	public static int Sign(this float parent)
	{
		return (int)Mathf.Sign(parent);
	}

	public static float Abs(this float parent)
	{
		return Mathf.Abs(parent);
	}

	public static int RoundToInt(this float parent)
	{
		return Mathf.RoundToInt(parent);
	}

	public static float RoundToTenth(this float parent)
	{
		return Mathf.RoundToInt(parent * 10f) / 10f;
	}
	#endregion

	#region Vector2
	public static Vector3 ToVector3(this Vector2 parent)
	{
		return parent.ToVector3(0f);
	}

	public static Vector3 ToVector3(this Vector2 parent, float z)
	{
		return new Vector3(parent.x, parent.y, z);
	}

	public static float RandomRange(this Vector2 parent)
	{
		return Random.Range(parent.x, parent.y);
	}
	#endregion

	#region Vector3
	public static Quaternion LookAt2D(this Vector3 parent, Vector3 target)
	{
		Vector3 targetPosition = target - parent;
		float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;

		return Quaternion.Euler(new Vector3(0f, 0f, Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles.z));
	}

	public static Vector3 DirectionToRotation2D(this Vector3 parent)
	{
		float angle = Mathf.Atan2(parent.y, parent.x) * Mathf.Rad2Deg;

		return Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles;
	}

	public static float DistanceFrom(this Vector3 parent, Vector3 target)
	{
		return Mathf.Sqrt(Mathf.Pow(parent.x - target.x, 2) + Mathf.Pow(parent.y - target.y, 2));
	}
	#endregion

	#region Component
	public static T[] GetInterfaceComponents<T>(this Component parent) where T : class
	{
		return System.Array.ConvertAll(parent.GetComponents(typeof(T)), c => c as T);
	}
	#endregion

	#region Transform
	public static void Flip(this Transform parent)
	{
		parent.localScale = new Vector3(-parent.localScale.x, parent.localScale.y, parent.localScale.z);
	}

	public static Vector3 TransformPointLocal(this Transform parent, Vector3 target)
	{
		return parent.TransformPoint(target) - parent.position;
	}

	public static void CorrectScaleForRotation(this Transform parent, Vector3 target, bool correctY = false)
	{
		bool flipY = target.z > 90f && target.z < 270f;

		target.y = correctY && flipY ? 180f : 0f;

		Vector3 newScale = parent.localScale;
		newScale.x = 1f;
		newScale.y = flipY ? -1f : 1f;
		parent.localScale = newScale;
		parent.rotation = Quaternion.Euler(target);
	}
	#endregion

	#region LayerMask
	public static bool ContainsLayer(this LayerMask parent, int layer)
	{
		return ((parent.value & (1 << layer)) > 0);
	}

	public static bool ContainsLayer(this LayerMask parent, GameObject obj)
	{
		return parent.ContainsLayer(obj.layer);
	}

	public static bool ContainsLayer(this LayerMask parent, Collider2D collider)
	{
		return parent.ContainsLayer(collider.gameObject.layer);
	}
	#endregion

	#region Generic Collection
	public static T GetRandom<T>(this IList<T> parent)
	{
		if (parent == null || parent.Count == 0) return default(T);

		return parent[Random.Range(0, parent.Count)];
	}

	public static T Pop<T>(this IList<T> parent)
	{
		if (parent == null || parent.Count == 0) return default(T);

		var lastIndex = parent.Count - 1;
		var lastItem = parent[lastIndex];

		parent.RemoveAt(lastIndex);

		return lastItem;
	}
	#endregion

	#region Utility
	public static int ClampWrap(int value, int min, int max)
	{
		if (value > max)
		{
			value = min;
		}
		else if (value < min)
		{
			value = max;
		}

		return value;
	}

	public static int RandomSign()
	{
		return (Random.value < 0.5) ? -1 : 1;
	}

	public static float ConvertRange(float num, float oldMin, float oldMax, float newMin, float newMax)
	{
		num = Mathf.Clamp(num, oldMin, oldMax);

		float oldRange = oldMax - oldMin;
		float newRange = newMax - newMin;

		return (((num - oldMin) * newRange) / oldRange) + newMin;
	}

	public static float GetDecimal(float num)
	{
		string result;

		if (num.ToString().Split('.').Length == 2)
		{
			result = "0." + num.ToString().Split('.')[1];
		}
		else
		{
			result = "0";
		}

		return float.Parse(result);
	}

	public static Vector3 SuperSmoothLerp(Vector3 followOld, Vector3 targetOld, Vector3 targetNew, float elapsedTime, float lerpAmount)
	{
		Vector3 f = followOld - targetOld + (targetNew - targetOld) / (lerpAmount * elapsedTime);
		return targetNew - (targetNew - targetOld) / (lerpAmount * elapsedTime) + f * Mathf.Exp(-lerpAmount * elapsedTime);
	}

	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;

		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}

	public static Vector3 Vector3Range(Vector3 min, Vector3 max)
	{
		return new Vector3(Random.Range(min.x, max.x),
						   Random.Range(min.y, max.y),
						   Random.Range(min.z, max.z));
	}

	public static float UnitsToPixels(float units)
	{
		var worldPoint = Camera.main.ViewportToWorldPoint(Vector3.zero) + new Vector3(units, 0f);
		return Camera.main.WorldToScreenPoint(worldPoint).x;
	}
	#endregion
}
