using System.Collections.Generic;

namespace PachowStudios.Assertions
{
  public class ICollectionAssertion<T> : ReferenceTypeAssertion<ICollection<T>, ICollectionAssertion<T>>
  {
    public ICollectionAssertion(ICollection<T> subject)
      : base(subject) { }

    public AndConstraint<ICollectionAssertion<T>> Contain(T item, string reason = null)
      => Assert(Subject.Contains(item), "contain", item.ToString(), reason);

    public AndConstraint<ICollectionAssertion<T>> NotContain(T item, string reason = null)
      => Assert(!Subject.Contains(item), "not contain", item.ToString(), reason);
  }
}