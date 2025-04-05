using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChainMapLib
{
    public class ChainMap<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly List<IDictionary<TKey, TValue>> _maps;

        public ChainMap(params IDictionary<TKey, TValue>[] maps)
        {
            _maps = new List<IDictionary<TKey, TValue>>();
            _maps.Add(new Dictionary<TKey, TValue>()); // Słownik główny

            if (maps != null)
            {
                // Dodaj słowniki w kolejności przekazanej (pierwszy parametr ma wyższy priorytet)
                foreach (var dict in maps)
                {
                    _maps.Add(new ReadOnlyDictionaryWrapper(dict));
                }
            }
        }

        public IDictionary<TKey, TValue> MainDictionary => _maps[0];

        public IList<IDictionary<TKey, TValue>> GetDictionaries()
            => _maps.Skip(1).ToList().AsReadOnly();

        public IDictionary<TKey, TValue> GetDictionary(int index)
        {
            if (index >= 0 && index < _maps.Count - 1)
                return _maps[index + 1];
            return null;
        }

        public void AddDictionary(IDictionary<TKey, TValue> dictionary, int index)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            var wrappedDict = new ReadOnlyDictionaryWrapper(dictionary);

            if (index < 0)
                _maps.Add(wrappedDict);
            else if (index >= _maps.Count - 1)
                _maps.Insert(1, wrappedDict);
            else
                _maps.Insert(index + 1, wrappedDict);
        }

        public bool RemoveDictionary(int index)
        {
            // Indeksy słowników pomocniczych: 0 do CountDictionaries - 1
            if (index < 0 || index >= CountDictionaries)
                return false;

            _maps.RemoveAt(index + 1); // +1, bo główny słownik jest na pozycji 0
            return true;
        }

        public void ClearDictionaries()
            => _maps.RemoveRange(1, _maps.Count - 1);

        public int CountDictionaries => _maps.Count - 1;

        // Implementacja metod IDictionary<TKey, TValue>
        public TValue this[TKey key]
        {
            get
            {
                foreach (var map in _maps)
                {
                    if (map.TryGetValue(key, out var value))
                        return value;
                }
                throw new KeyNotFoundException($"Klucz '{key}' nie istnieje.");
            }
            set => MainDictionary[key] = value;
        }

        public ICollection<TKey> Keys
            => _maps.SelectMany(d => d.Keys).Distinct().ToList();

        public ICollection<TValue> Values
            => Keys.Select(k => this[k]).ToList();

        public int Count => Keys.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (MainDictionary.ContainsKey(key))
                throw new ArgumentException("Klucz już istnieje w słowniku głównym.");
            MainDictionary.Add(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (MainDictionary.ContainsKey(key))
                return false;
            MainDictionary.Add(key, value);
            return true;
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var map in _maps)
            {
                if (map.ContainsKey(key))
                    return true;
            }
            return false;
        }

        public bool Remove(TKey key)
            => MainDictionary.Remove(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var map in _maps)
            {
                if (map.TryGetValue(key, out value))
                    return true;
            }
            value = default;
            return false;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

        public void Clear()
            => MainDictionary.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _maps.Any(d => d.Contains(item));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var items = this.ToList();
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var yieldedKeys = new HashSet<TKey>();
            foreach (var map in _maps)
            {
                foreach (var kvp in map)
                {
                    if (yieldedKeys.Add(kvp.Key))
                        yield return kvp;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public Dictionary<TKey, TValue> Merge()
        {
            var merged = new Dictionary<TKey, TValue>();
            foreach (var map in _maps)
            {
                foreach (var kvp in map)
                {
                    if (!merged.ContainsKey(kvp.Key))
                        merged.Add(kvp.Key, kvp.Value);
                }
            }
            return merged;
        }

        private class ReadOnlyDictionaryWrapper : IDictionary<TKey, TValue>
        {
            private readonly IDictionary<TKey, TValue> _dict;

            public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> dict)
                => _dict = dict;

            // Implementacja metod IDictionary<TKey, TValue> z blokadą zapisu
            public TValue this[TKey key] { get => _dict[key]; set => throw new NotSupportedException(); }
            public ICollection<TKey> Keys => _dict.Keys;
            public ICollection<TValue> Values => _dict.Values;
            public int Count => _dict.Count;
            public bool IsReadOnly => true;
            public void Add(TKey key, TValue value) => throw new NotSupportedException();
            public void Add(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(KeyValuePair<TKey, TValue> item) => _dict.Contains(item);
            public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dict.CopyTo(array, arrayIndex);
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
            public bool Remove(TKey key) => throw new NotSupportedException();
            public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();
            public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
            IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();
        }
    }
}