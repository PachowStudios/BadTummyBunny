using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PachowStudios.Collections
{
  public interface IReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
  {
    int Count { get; }
    [NotNull] TValue this[TKey key] { get; }
    [NotNull] IEnumerable<TKey> Keys { get; }
    [NotNull] IEnumerable<TValue> Values { get; }

    [Pure]
    bool ContainsKey([NotNull] TKey key);

    [Pure, ContractAnnotation("=>true; =>false, value: null")]
    bool TryGetValue([NotNull] TKey key, out TValue value);
  }
}