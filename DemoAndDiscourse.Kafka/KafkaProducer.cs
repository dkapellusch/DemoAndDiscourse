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
        private readonly string _topicName;
        private readonly IProducer<TKey, TPayload> _producer;

        public KafkaProducer(ProducerConfig config, IMessageSerializer<TPayload> serializer, string topicName = null)
        {
            _topicName = topicName ?? $"{typeof(TPayload).Name}s";
            _producer = new ProducerBuilder<TKey, TPayload>(config)
                .SetValueSerializer(new KafkaSerializer<TPayload>(serializer))
                .Build();
        }

        public async Task<DeliveryResult<TKey, TPayload>> ProduceAsync(TPayload payload, TKey key)
        {
            var message = new Message<TKey, TPayload> {Key = key, Value = payload};
            return await _producer.ProduceAsync(_topicName, message);
        }

        public void Flush(CancellationToken token = default) => _producer.Flush(token);

        public void Dispose() => _producer?.Dispose();
    }
}