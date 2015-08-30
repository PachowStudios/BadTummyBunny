using System.Collections.Generic;
using UnityEngine;

public class FixedSizedVector3Queue
{
	private List<Vector3> list;
	private int limit;

	public FixedSizedVector3Queue(int limit)
	{
		this.limit = limit;
		list = new List<Vector3>(limit);
	}

	public void Push(Vector3 item)
	{
		if (list.Count == limit)
			list.RemoveAt(0);

		list.Add(item);
	}

	public Vector3 Average()
	{
		var avg = Vector3.zero;

		// early out for no items
		if (list.Count == 0)
			return avg;

		for (var i = 0; i < list.Count; i++)
			avg += list[i];

		return avg / list.Count;
	}
}