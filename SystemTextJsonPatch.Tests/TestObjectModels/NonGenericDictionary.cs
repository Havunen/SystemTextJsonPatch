using System;
using System.Collections;
using System.Collections.Generic;

namespace SystemTextJsonPatch.IntegrationTests;

public class NonGenericDictionary : IDictionary
{
	private readonly Dictionary<object, object> _dictionary = new();

	public object this[object key] { get => _dictionary[key]; set => _dictionary[key] = value; }

	public bool IsFixedSize => false;

	public bool IsReadOnly => false;

	public ICollection Keys => _dictionary.Keys;

	public ICollection Values => _dictionary.Values;

	public int Count => _dictionary.Count;

	public bool IsSynchronized => false;

	public object SyncRoot => null;

	public void Add(object key, object value) => _dictionary.Add(key, value);

	public void Clear() => _dictionary.Clear();

	public bool Contains(object key) => _dictionary.ContainsKey(key);

	public void CopyTo(Array array, int index) => throw new NotImplementedException();

	public IDictionaryEnumerator GetEnumerator() => _dictionary.GetEnumerator();

	public void Remove(object key) => _dictionary.Remove(key);

	IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
}

