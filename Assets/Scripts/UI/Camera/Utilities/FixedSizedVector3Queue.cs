using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class FixedSizedVector3Queue
  {
    private List<Vector3> List { get; }
    private int Limit { get; }

    public FixedSizedVector3Queue(int limit)
    {
      Limit = limit;
      List = new List<Vector3>(limit);
    }

    public void Push(Vector3 item)
    {
      if (List.Count == Limit)
        List.RemoveAt(0);

      List.Add(item);
    }

    public Vector3 Average()
    {
      if (List.IsEmpty())
        return Vector3.zero;

      var avg = List.Aggregate(Vector3.zero, (current, t) => current + t);

      return avg / List.Count;
    }
  }
}
