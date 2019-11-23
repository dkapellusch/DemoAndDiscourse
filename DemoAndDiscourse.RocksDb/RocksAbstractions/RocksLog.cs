using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using DemoAndDiscourse.RocksDb.Serialization;

namespace DemoAndDiscourse.RocksDb.RocksAbstractions
{
    public class RocksLog<T> : IReadOnlyCollection<(long offset, T value)>
    {
        private readonly int MaxKeyLength = long.MaxValue.ToString().Length;
        private readonly RocksStore _rocksStore;
        private readonly ISerializer _serializer;
        private long _currentCount;

        public RocksLog(RocksStore rocksStore, ISerializer serializer)
        {
            _rocksStore = rocksStore;
            _serializer = serializer;
            _currentCount = GetLastKey();
        }

        public long CurrentCount => _currentCount + 1;

        public void Write(T item) => _rocksStore.Add(GetNextKey(), item);

        public T Read(long offset) => _rocksStore.Get<string, T>(OffsetToKey(offset));

        public IObservable<(long offset, T value)> ReadFrom(long offset)
            => _rocksStore.GetItems<string, T>(OffsetToKey(offset))
                .Select(kv => (long.Parse(kv.key), kv.value))
                .ToObservable()
                .Concat(_rocksStore.ChangedDataCaptureStream()
                    .OfType<DataChangedEvent<string, T>>()
                    .Select(kv => (long.Parse(kv.Data.key), kv.Data.value))
                );

        public T this[long offset] => Read(offset);

        public IEnumerator<(long offset, T value)> GetEnumerator()
            => _rocksStore.GetItems<string, T>(OffsetToKey(0))
                .Select(kv => (long.Parse(kv.key), kv.value))
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private string OffsetToKey(long offset) => offset.ToString().PadLeft(MaxKeyLength, '0');

        private string GetNextKey() => OffsetToKey(Interlocked.Increment(ref _currentCount));

        private long GetLastKey()
        {
            using var iterator = _rocksStore.GetIterator<T>();
            iterator.SeekToLast();

            if (iterator.Valid())
            {
                var key = _serializer.Deserialize<string>(iterator.Key());
                return long.Parse(key);
            }

            return -1;
        }

        public int Count => unchecked((int) CurrentCount);
    }
}