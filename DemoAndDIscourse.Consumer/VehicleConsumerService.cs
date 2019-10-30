using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.Consumer
{
    public sealed class VehicleConsumerService : IHostedService
    {
        private readonly KafkaConsumer<Vehicle> _consumer;

        public VehicleConsumerService(KafkaConsumer<Vehicle> consumer)
        {
            _consumer = consumer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Start(cancellationToken);

            await _consumer.Subscription.ForEachAsync(message =>
                {
                    Console.WriteLine($"[{message.Topic}:({message.Partition.Value}, {message.Offset.Value})]: {message.Message.Value}");

                    _consumer.Commit(message.Partition, message.Offset);
                },
                cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}