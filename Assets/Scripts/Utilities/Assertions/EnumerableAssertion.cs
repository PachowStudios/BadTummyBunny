using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;

namespace PachowStudios.Assertions
{
  public class EnumerableAssertion<T> : BaseAssertion<IEnumerable<T>>
  {
    public EnumerableAssertion(IEnumerable<T> subject)
      : base(subject) { }

    public void BeEmpty(string reason = null)
      => Assert(Subject.IsEmpty(), "be", "empty", reason);

    public void NotBeEmpty(string reason = null)
      => Assert(Subject.Any(), "not be", "empty", reason);
  }
}