using System;
using System.Collections;
using UnityEngine;

public class Wait
{
	private static WaitInternal instance;

	private static WaitInternal Instance
	{
		get
		{
			if (instance == null)
				instance = CreateInstance();

			return instance;
		}
	}

	public static void ForSeconds(float waitTime, Action callback) => Instance.StartCoroutine(Instance.ForSecondsCoroutine(waitTime, callback));

	public static void ForRealSeconds(float waitTime, Action callback) =>Instance.StartCoroutine(Instance.ForRealSecondsCoroutine(waitTime, callback));

	public static void ForFixedUpdate(Action callback) => Instance.StartCoroutine(Instance.ForFixedUpdateCoroutine(callback));

	public static void ForEndOfFrame(Action callback) => Instance.StartCoroutine(Instance.ForEndOfFrameCoroutine(callback));

	private static WaitInternal CreateInstance()
	{
		var newInstance = new GameObject("Wait Utility");

		newInstance.HideInHierarchy();

		return newInstance.AddComponent<WaitInternal>();
	}

	private class WaitInternal : MonoBehaviour
	{
		public IEnumerator ForSecondsCoroutine(float waitTime, Action callback)
		{
			yield return new WaitForSeconds(waitTime);

			callback?.Invoke();
		}

		public IEnumerator ForRealSecondsCoroutine(float waitTime, Action callback)
		{
			var startTime = Time.realtimeSinceStartup;

			while (Time.realtimeSinceStartup < startTime + waitTime)
				yield return null;

			callback?.Invoke();
		}

		public IEnumerator ForFixedUpdateCoroutine(Action callback)
		{
			yield return new WaitForFixedUpdate();

			callback?.Invoke();
		}

		public IEnumerator ForEndOfFrameCoroutine(Action callback)
		{
			yield return new WaitForEndOfFrame();

			callback?.Invoke();
		}
	}
}