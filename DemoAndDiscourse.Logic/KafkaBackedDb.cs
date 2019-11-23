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
        private readonly KafkaConsumer<TValue> _ksqlConsumer;

        public KafkaBackedDb(RocksDictionary<string, TValue> dictionary, KafkaConsumer<TValue> ksqlConsumer)
        {
            _dictionary = dictionary;
            _ksqlConsumer = ksqlConsumer;

            _ksqlConsumer.Start();
            _ksqlConsumer.Subscription.ObserveOn(TaskPoolScheduler.Default).SubscribeOn(TaskPoolScheduler.Default).Subscribe(m => _dictionary[m.Key] = m.Value);
        }

        public TValue GetItem(string key) => _dictionary[key];

        public IEnumerable<TValue> GetAll() => _dictionary.Values;

        public IObservable<TValue> GetChanges() => _dictionary.DataChanges.Select(d => d.Data.value);
    }
}