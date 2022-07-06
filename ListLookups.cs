#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GraphAlgorithmTester;

public class ListLookups<TKey, TValue> 
    : Lookups<TKey,TValue> where TKey:notnull {
    public bool HasChangedWithOrder(ListLookups<TKey, TValue> that)
        => that == null
        || this.Count != that.Count
        || this.Any(p => !that.ContainsKey(p.Key)
        || !this[p.Key].SequenceEqual(that[p.Key]));
    public bool HasChangedWithoutOrder(ListLookups<TKey, TValue> that)
        => that == null
        || this.Count != that.Count
        || this.Any(p => !that.ContainsKey(p.Key)
        || !this[p.Key].ToHashSet().SetEquals(that[p.Key]));
    public Dictionary<TKey, ICollection<TValue>> Data { get; protected set; } = new();
    public int Count => this.Data.Count;
    public bool IsEmpty => this.Data.Count == 0;
    public int TotalValuesCount => this.Values.Sum(v => v.Count);
    public ListLookups() { }
    public ListLookups(IEnumerable<KeyValuePair<TKey,ICollection<TValue>>> collection)
    {
        foreach(var c in collection)
            foreach(var v in c.Value)
                this[c.Key].Add(v);
    }
    public ListLookups<TKey, TValue> Clone() 
        => new(this.Data);
    public ListLookups<TKey, TValue> Transfer()
    {
        var t = new ListLookups<TKey, TValue>(this.Data);
        this.Clear();
        return t;
    }
    public override string ToString() => $"KeyType={typeof(TKey).Name},ValueType={typeof(TValue).Name},Count={this.Count}";
    public Dictionary<TKey, ICollection<TValue>>.KeyCollection Keys => this.Data.Keys;
    public Dictionary<TKey, ICollection<TValue>>.ValueCollection Values => this.Data.Values;
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, ICollection<TValue>>>)Data).IsReadOnly;
    ICollection<TKey> IDictionary<TKey, ICollection<TValue>>.Keys => ((IDictionary<TKey, ICollection<TValue>>)Data).Keys;
    ICollection<ICollection<TValue>> IDictionary<TKey, ICollection<TValue>>.Values => ((IDictionary<TKey, ICollection<TValue>>)Data).Values;
    public bool IsSynchronized => ((ICollection)Data).IsSynchronized;
    public object SyncRoot => ((ICollection)Data).SyncRoot;
    public bool IsFixedSize => ((IDictionary)Data).IsFixedSize;
    ICollection<TValue> IDictionary<TKey, ICollection<TValue>>.this[TKey key] { get => ((IDictionary<TKey, ICollection<TValue>>)Data)[key]; set => ((IDictionary<TKey, ICollection<TValue>>)Data)[key] = value; }
    public bool ContainsKey(TKey key) => this.Data.ContainsKey(key);
    public bool ContainsList(ICollection<TValue> list) => this.Data.ContainsValue(list);
    public bool ContainsValue(TValue value) => this.Data.Values.SelectMany(s=>s).Contains(value);
    public void Add(TKey key, TValue value) 
    {
        if (this.Data.TryGetValue(key, out var list))
            list.Add(value);
        else
            this.Data.Add(key, new List<TValue> { value });
    }
    public void Add(TKey key, IEnumerable<TValue> values)
    {
        if (this.Data.TryGetValue(key, out var list))
            (list as List<TValue>)?.AddRange(values);
        else
            this.Data.Add(key, new List<TValue>(values));
    }
    public ICollection<TValue> this[TKey key]
    {
        get
        {
            if(!this.Data.TryGetValue(key, out var list))
                this.Data.Add(key, list = new List<TValue>());
            return list;
        }
        set
        {
            foreach(var v in value)
                this[key].Add(v);                
        }
    }
    public bool Remove(TKey key) => this.Data.Remove(key);
    public void Clear() => this.Data.Clear();
    IEnumerator IEnumerable.GetEnumerator() => this.Data.GetEnumerator();
    public IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>)Data).GetEnumerator();
    public void Add(KeyValuePair<TKey, ICollection<TValue>> item)
        => ((ICollection<KeyValuePair<TKey, ICollection<TValue>>>)Data).Add(item);
    public bool Contains(KeyValuePair<TKey, ICollection<TValue>> item) => ((ICollection<KeyValuePair<TKey, ICollection<TValue>>>)Data).Contains(item);
    public void CopyTo(KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, ICollection<TValue>>>)Data).CopyTo(array, arrayIndex);
    public bool Remove(KeyValuePair<TKey, ICollection<TValue>> item) => ((ICollection<KeyValuePair<TKey, ICollection<TValue>>>)Data).Remove(item);
    public void Add(TKey key, ICollection<TValue> value) => ((IDictionary<TKey, ICollection<TValue>>)Data).Add(key, value);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out ICollection<TValue> value) => ((IDictionary<TKey, ICollection<TValue>>)Data).TryGetValue(key, out value);
    public void CopyTo(Array array, int index) => ((ICollection)Data).CopyTo(array, index);
    public void Add(object key, object? value) => ((IDictionary)Data).Add(key, value);
    public bool Contains(object key) => ((IDictionary)Data).Contains(key);
    public void Remove(object key) => ((IDictionary)Data).Remove(key);
}
