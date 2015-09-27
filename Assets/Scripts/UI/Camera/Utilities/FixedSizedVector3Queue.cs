using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FixedSizedVector3Queue
{
  private List<Vector3> list;
  private int limit;

  public FixedSizedVector3Queue(int limit)
  {
    this.limit = limit;
    this.list = new List<Vector3>(limit);
  }

  public void Push(Vector3 item)
  {
    if (this.list.Count == this.limit)
      this.list.RemoveAt(0);

    this.list.Add(item);
  }

  public Vector3 Average()
  {
    if (this.list.Count == 0)
      return Vector3.zero;

    var avg = this.list.Aggregate(Vector3.zero, (current, t) => current + t);

    return avg / this.list.Count;
  }
}
