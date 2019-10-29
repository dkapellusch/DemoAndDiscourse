using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Confluent.Kafka;
using DemoAndDiscourse.Utils;
using Google.Protobuf;

namespace DemoAndDiscourse.Kafka
{
    public sealed class KafkaConsumer<TPayload> : IDisposable where TPayload : IMessage<TPayload>, new()
    {
        private readonly IConsumer<string, TPayload> _consumer;
        private readonly string _topicName;

        public KafkaConsumer(ConsumerConfig config, IMessageSerializer<TPayload> serializer, string topicName = null)
        {
            _topicName = topicName ?? $"{typeof(TPayload).Name}s";
            _consumer = new ConsumerBuilder<string, TPayload>(config)
                .SetValueDeserializer(new KafkaSerializer<TPayload>(serializer))
                .Build();
        }

        public IObservable<ConsumeResult<string, TPayload>> Subscription { get; set; }

        public void Dispose()
        {
            _consumer?.Dispose();
        }

        public void Start(CancellationToken token = default)
        {
            if (!_consumer.Subscription.Contains(_topicName)) _consumer.Subscribe(_topicName);

            if (Subscription != null) return;

            Subscription =
                Observable.Start(() => ReadOne(token))
                    .Expand(lv => Observable.Start(() => ReadOne(token)))
                    .Where(m => m.IsNotNullOrDefault())
                    .TakeWhile(r => !token.IsCancellationRequested);
        }

        public void SeekToTime(DateTime time)
        {
            var offset = _consumer.OffsetsForTimes(new[] {new TopicPartitionTimestamp(_topicName, Partition.Any, new Timestamp(time))}, TimeSpan.FromSeconds(30));
            var firstOffset = offset.FirstOrDefault();

            if (firstOffset is null) return;

            _consumer.Seek(firstOffset);
        }

        public bool Commit(int partition, long offset)
        {
            try
            {
                _consumer.Commit(new[] {new TopicPartitionOffset(_topicName, partition, offset + 1)});

                return true;
            }
            catch
            {
                return false;
            }
        }

        private ConsumeResult<string, TPayload> ReadOne(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(90));

                    if (consumeResult?.Message is null || consumeResult.IsPartitionEOF || consumeResult.Value is null) continue;

                    return consumeResult;
                }
                catch (ConsumeException)
                {
                }
            }

            return default;
        }
    }
}