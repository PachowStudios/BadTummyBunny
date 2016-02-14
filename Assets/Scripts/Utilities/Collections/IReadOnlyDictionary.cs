using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PachowStudios.Collections
{
  public interface IReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
  {
    int Count { get; }
    TValue this[TKey key] { get; }
    [NotNull] IEnumerable<TKey> Keys { get; }
    [NotNull] IEnumerable<TValue> Values { get; }

    bool ContainsKey(TKey key);
    bool TryGetValue(TKey key, out TValue value);
  }
}