using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;

namespace PachowStudios.Assertions
{
  public class IEnumerableAssertion<T> : ReferenceTypeAssertion<IEnumerable<T>, IEnumerableAssertion<T>>
  {
    public IEnumerableAssertion(IEnumerable<T> subject)
      : base(subject) { }

    public AndConstraint<IEnumerableAssertion<T>> BeEmpty(string reason = null)
      => Assert(Subject.IsEmpty(), "be", "empty", reason);

    public AndConstraint<IEnumerableAssertion<T>> NotBeEmpty(string reason = null)
      => Assert(Subject.Any(), "not be", "empty", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveNoneWhere(Func<T, bool> condition, string reason = null)
      => Assert(Subject.None(condition), "have none where", $"{condition} is true", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveAtLeast(int amount, string reason = null)
      => Assert(Subject.HasAtLeast(amount), "have at lesast", $"{amount} items", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveMoreThan(int amount, string reason = null)
      => Assert(Subject.HasMoreThan(amount), "have more than", $"{amount} items", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveAtMost(int amount, string reason = null)
      => Assert(Subject.HasAtMost(amount), "have at most", $"{amount} items", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveLessThan(int amount, string reason = null)
      => Assert(Subject.HasLessThan(amount), "have less than", $"{amount} items", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveExactly(int amount, string reason = null)
      => Assert(Subject.HasExactly(amount), "have exactly", $"{amount} items", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveSingleItem(string reason = null)
      => Assert(Subject.HasSingle(), "have", "a single item", reason);

    public AndConstraint<IEnumerableAssertion<T>> HaveMultipleItems(string reason = null)
      => Assert(Subject.HasMultiple(), "have", "multiple items", reason);
  }
}