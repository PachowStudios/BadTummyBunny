using System.Collections.Generic;

namespace PachowStudios.Assertions
{
  public class IDictionaryAssertion<TKey, TValue> : ReferenceTypeAssertion<IDictionary<TKey, TValue>, IDictionaryAssertion<TKey, TValue>>
  {
    public IDictionaryAssertion(IDictionary<TKey, TValue> subject)
      : base(subject) { }

    public AndConstraint<IDictionaryAssertion<TKey, TValue>> ContainKey(TKey key, string reason = null)
      => Assert(Subject.ContainsKey(key), "contain key", key.ToString(), reason);
  }
}