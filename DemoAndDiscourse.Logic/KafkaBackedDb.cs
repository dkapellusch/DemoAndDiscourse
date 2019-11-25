using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.RocksDb.RocksAbstractions;
using Google.Protobuf;

namespace DemoAndDiscourse.Logic
{
    public class KafkaBackedDb<TValue> where TValue : IMessage<TValue>
    {
        private readonly RocksStore _rocksStore;

        public KafkaBackedDb(RocksStore rocksStore, KafkaConsumer<TValue> kafkaConsumer)
        {
            _rocksStore = rocksStore;

            kafkaConsumer.Start();
            kafkaConsumer.Subscription
                .ObserveOn(TaskPoolScheduler.Default)
                .SubscribeOn(TaskPoolScheduler.Default)
                .Subscribe(m =>
                {
                    var value = m.Value;
                    var currentValue = _rocksStore.Get<string, TValue>(m.Key);

                    if (!(currentValue is null))
                        value.UpdateObject(currentValue);

                    _rocksStore.Add(m.Key, value);
                    kafkaConsumer.Commit(m.Partition, m.Offset);
                });
        }

        public TValue GetItem(string key) => _rocksStore.Get<string, TValue>(key);

        public IEnumerable<TValue> GetAll() => _rocksStore.GetItems<string, TValue>().Select(pair => pair.value);

        public IEnumerable<TValue> GetItems(string key) => _rocksStore.GetItems<string, TValue>(key, (k, _) => k.Contains(key)).Select(kv => kv.value);

        public IObservable<TValue> GetChanges() => _rocksStore.ChangedDataCaptureStream().OfType<DataChangedEvent<string, TValue>>().Select(dc => dc.Data.value);
    }
}