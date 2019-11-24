using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.RocksDb.RocksAbstractions;
using Google.Protobuf;

namespace DemoAndDiscourse.Logic
{
    public class KafkaBackedDb<TValue> where TValue : IMessage<TValue>
    {
        private readonly RocksDictionary<string, TValue> _dictionary;

        public KafkaBackedDb(RocksDictionary<string, TValue> dictionary, KafkaConsumer<TValue> ksqlConsumer)
        {
            _dictionary = dictionary;

            ksqlConsumer.Start();
            ksqlConsumer.Subscription
                .ObserveOn(TaskPoolScheduler.Default)
                .SubscribeOn(TaskPoolScheduler.Default)
                .Subscribe(m =>
                {
                    var value = m.Value;

                    if (_dictionary.TryGetValue(m.Key, out var currentElement))
                        value = value.UpdateObject(currentElement);

                    _dictionary[m.Key] = value;
                    ksqlConsumer.Commit(m.Partition, m.Offset);
                });
        }

        public TValue GetItem(string key) => _dictionary[key];

        public IEnumerable<TValue> GetAll() => _dictionary.Values;

//        public IObservable<TValue> GetChanges() => _dictionary.DataChanges.Select(d => d.Data.value);
        public IObservable<TValue> GetChanges() => _dictionary.DataChanges.Select(dc => dc.Data.value);
    }
}