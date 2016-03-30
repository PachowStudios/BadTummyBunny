using System.Collections.Generic;

namespace PachowStudios.Assertions
{
  public class CollectionAssertion<T> : BaseAssertion<ICollection<T>>
  {
    public CollectionAssertion(ICollection<T> subject)
      : base(subject) { }

    public void Contain(T item, string reason = null)
      => Assert(Subject.Contains(item), "contain", item.ToString(), reason);

    public void NotContain(T item, string reason = null)
      => Assert(!Subject.Contains(item), "not contain", item.ToString(), reason);
  }
}