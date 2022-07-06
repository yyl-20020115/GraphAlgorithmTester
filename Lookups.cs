using System.Collections;
using System.Collections.Generic;

namespace GraphAlgorithmTester;

public interface Lookups<TKey, TValue>
    : IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>,
      ICollection<KeyValuePair<TKey, ICollection<TValue>>>,
      IDictionary<TKey, ICollection<TValue>>,
      IEnumerable,
      ICollection
    where TKey : notnull { }
