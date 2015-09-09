using System;
using System.Collections;
using UnityEngine;

public class Wait : MonoBehaviour
{
	private static Wait instance;

	private static Wait Instance
	{
		get
		{
			if (instance == null)
				instance = new Wait();

			return instance;
		}
	}

	public static void ForSeconds(float waitTime, Action callback)
	{
		Instance.StartCoroutine(Instance.ForSecondsCoroutine(waitTime, callback));
	}

	public static void ForFixedUpdate(Action callback)
	{
		Instance.StartCoroutine(Instance.ForFixedUpdateCoroutine(callback));
	}

	public static void ForEndOfFrame(Action callback)
	{
		Instance.StartCoroutine(Instance.ForEndOfFrameCoroutine(callback));
	}

	private IEnumerator ForSecondsCoroutine(float waitTime, Action callback)
	{
		yield return new WaitForSeconds(waitTime);

		var tempCallback = callback;

		if (tempCallback != null) tempCallback();
	}

	private IEnumerator ForFixedUpdateCoroutine(Action callback)
	{
		yield return new WaitForFixedUpdate();

		var tempCallback = callback;

		if (tempCallback != null) tempCallback();
	}

	private IEnumerator ForEndOfFrameCoroutine(Action callback)
	{
		yield return new WaitForEndOfFrame();

		var tempCallback = callback;

		if (tempCallback != null) tempCallback();
	}
}