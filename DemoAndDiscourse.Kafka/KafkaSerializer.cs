using System;
using Confluent.Kafka;
using DemoAndDiscourse.Utils;

namespace DemoAndDiscourse.Kafka
{
    internal sealed class KafkaSerializer<TMessage> : ISerializer<TMessage>, IDeserializer<TMessage>
    {
        private readonly IMessageSerializer<TMessage> _messageSerializer;

        public KafkaSerializer(IMessageSerializer<TMessage> messageSerializer)
        {
            _messageSerializer = messageSerializer;
        }

        public TMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            => isNull || data.IsEmpty || data.Length <= 0
                ? default
                : _messageSerializer.Deserialize(data.ToArray());

        public byte[] Serialize(TMessage data, SerializationContext context)
            => data is null
                ? null
                : _messageSerializer.Serialize(data);
    }
}