using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DemoAndDiscourse.RocksDb.RocksAbstractions
{
    public class RocksDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly RocksStore _rocksStore;

        public RocksDictionary(RocksStore rocksStore)
        {
            _rocksStore = rocksStore;
        }

        public IObservable<DataChangedEvent<TKey, TValue>> DataChanges => _rocksStore.ChangedDataCaptureStream().Select(m => m).OfType<DataChangedEvent<TKey, TValue>>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _rocksStore.GetItems<TKey, TValue>().Select(kv => new KeyValuePair<TKey, TValue>(kv.key, kv.value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _rocksStore.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            foreach (var (key, _) in _rocksStore.GetItems<TKey, TValue>())
            {
                _rocksStore.Delete<TKey, TValue>(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var keyExists = TryGetValue(item.Key, out var element);
            return keyExists && Equals(item, element);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var index = arrayIndex;

            foreach (var (key, value) in this)
            {
                array[index++] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _rocksStore.Delete<TKey, TValue>(item.Key);
            return true;
        }

        public int Count => _rocksStore.GetItems<TKey, TValue>().Count();

        public bool IsReadOnly { get; } = false;

        public void Add(TKey key, TValue value)
        {
            _rocksStore.Add(key, value);
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public bool Remove(TKey key)
        {
            _rocksStore.Delete<TKey, TValue>(key);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = this[key];

            return !(value is null);
        }

        public TValue this[TKey key]
        {
            get => _rocksStore.Get<TKey, TValue>(key);
            set => _rocksStore.Add(key, value);
        }

        public ICollection<TKey> Keys => _rocksStore.GetItems<TKey, TValue>().Select(kv => kv.key).ToArray();

        public ICollection<TValue> Values => _rocksStore.GetItems<TKey, TValue>().Select(kv => kv.value).ToArray();
    }
}