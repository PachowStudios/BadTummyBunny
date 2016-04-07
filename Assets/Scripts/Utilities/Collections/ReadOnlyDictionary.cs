using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PachowStudios.Collections
{
  public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
  {
    private readonly IDictionary<TKey, TValue> dictionary;

    public int Count => this.dictionary.Count;
    public TValue this[TKey key] => this.dictionary[key];
    public IEnumerable<TKey> Keys => this.dictionary.Keys;
    public IEnumerable<TValue> Values => this.dictionary.Values;

    public ReadOnlyDictionary([NotNull] IDictionary<TKey, TValue> dictionary)
    {
      this.dictionary = dictionary;
    }

    public bool ContainsKey(TKey key)
      => this.dictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value)
      => this.dictionary.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
      => this.dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }

  public static class ReadOnlyDictionaryExtensions
  {
    [NotNull, Pure]
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary)
      => new ReadOnlyDictionary<TKey, TValue>(dictionary);
  }
}