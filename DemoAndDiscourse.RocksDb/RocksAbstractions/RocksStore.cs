using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using DemoAndDiscourse.RocksDb.Extensions;
using DemoAndDiscourse.RocksDb.Serialization;
using RocksDbSharp;

namespace DemoAndDiscourse.RocksDb.RocksAbstractions
{
    public class RocksStore
    {
        private readonly RocksDatabase _db;
        private readonly ISerializer _serializer;
        private readonly Subject<object> _subject;
        private IObservable<object> _stream;

        public RocksStore(RocksDatabase db, ISerializer serializer)
        {
            _db = db;
            _serializer = serializer;
            _subject = new Subject<object>();
        }

        private RocksDbSharp.RocksDb RocksDb => _db.RocksDb;

        public void Add<TKey, TValue>(TKey key, TValue itemToAdd)
        {
            RocksDb.Put(_serializer.Serialize(key), _serializer.Serialize(itemToAdd), GetColumnFamily<TValue>());

            var addEvent = new DataChangedEvent<TKey, TValue>(Operation.DataUpdated, (key, itemToAdd));
            WriteToAuditLog(addEvent);
            _subject.OnNext(addEvent);
        }

        public TValue Get<TKey, TValue>(TKey key)
        {
            var item = RocksDb.Get(_serializer.Serialize(key), GetColumnFamily<TValue>());
            return _serializer.Deserialize<TValue>(item);
        }

        public void Delete<TKey, TValue>(TKey key)
        {
            RocksDb.Remove(_serializer.Serialize(key), GetColumnFamily<TValue>());

            var deletedEvent = new DataChangedEvent<TKey, TValue>(Operation.DataDeleted, (key, default));
            WriteToAuditLog(deletedEvent);
            _subject.OnNext(deletedEvent);
        }

        public IEnumerable<(TKey key, TValue value)> GetItems<TKey, TValue>(TKey key, Func<TKey, TValue, bool> condition)
        {
            var startingKey = _serializer.Serialize(key);
            var iteratorOptions = new ReadOptions();

            return RocksDb.NewIterator(GetColumnFamily<TValue>(), iteratorOptions)
                .Seek(startingKey)
                .GetEnumerable((serializedKey, serializedValue) => condition(_serializer.Deserialize<TKey>(serializedKey), _serializer.Deserialize<TValue>(serializedValue)))
                .Select(kv => (_serializer.Deserialize<TKey>(kv.key), _serializer.Deserialize<TValue>(kv.value)));
        }

        public IEnumerable<(TKey key, TValue value)> GetItems<TKey, TValue>(TKey key)
        {
            var startingKey = _serializer.Serialize(key);
            var iteratorOptions = new ReadOptions();

            return RocksDb.NewIterator(GetColumnFamily<TValue>(), iteratorOptions)
                .Seek(startingKey)
                .GetEnumerable()
                .Select(kv => (_serializer.Deserialize<TKey>(kv.key), _serializer.Deserialize<TValue>(kv.value)));
        }

        public IEnumerable<(TKey key, TValue value)> GetItems<TKey, TValue>()
        {
            var iteratorOptions = new ReadOptions();

            return RocksDb.NewIterator(GetColumnFamily<TValue>(), iteratorOptions)
                .SeekToFirst()
                .GetEnumerable()
                .Select(kv => (_serializer.Deserialize<TKey>(kv.key), _serializer.Deserialize<TValue>(kv.value)));
        }

        public Iterator GetIterator<TValue>() => RocksDb.NewIterator(GetColumnFamily<TValue>(), new ReadOptions());

        public IObservable<object> ChangedDataCaptureStream() => _stream ??= _subject;

        private void WriteToAuditLog<TKey, TValue>(DataChangedEvent<TKey, TValue> dataChangedEvent) =>
            RocksDb.Put(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), _serializer.Serialize(dataChangedEvent), GetAuditColumnFamily<TValue>());

        private ColumnFamilyHandle GetColumnFamily<T>() => _db.GetOrCreateColumnFamily(typeof(T).Name);

        private ColumnFamilyHandle GetAuditColumnFamily<T>() => _db.GetOrCreateColumnFamily($"Audit/{typeof(T).Name}");
    }
}