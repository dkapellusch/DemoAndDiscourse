using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using DemoAndDiscourse.Utils;
using Google.Protobuf;

namespace DemoAndDiscourse.Kafka
{
    public sealed class KafkaProducer<TKey, TPayload> : IDisposable where TPayload : IMessage<TPayload>, new()
    {
        private readonly IProducer<TKey, TPayload> _producer;
        private readonly string _topicName;

        public KafkaProducer(ProducerConfig config, IMessageSerializer<TPayload> serializer, string topicName = null)
        {
            _topicName = topicName ?? $"{typeof(TPayload).Name}s";
            _producer = new ProducerBuilder<TKey, TPayload>(config)
                .SetValueSerializer(new KafkaSerializer<TPayload>(serializer))
                .Build();
        }

        public void Dispose() => _producer?.Dispose();

        public async Task<DeliveryResult<TKey, TPayload>> ProduceAsync(TPayload payload, TKey key) =>
            await _producer.ProduceAsync(_topicName, new Message<TKey, TPayload> {Key = key, Value = payload});

        public void Flush(CancellationToken token = default) => _producer.Flush(token);
    }
}