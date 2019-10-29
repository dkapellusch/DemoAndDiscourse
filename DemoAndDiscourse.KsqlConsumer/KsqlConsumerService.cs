using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka.Ksql;
using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.KsqlConsumer
{
    public sealed class KsqlConsumerService : IHostedService
    {
        private readonly KafkaKsqlConsumer<Vehicle> _ksqlConsumer;

        public KsqlConsumerService(KafkaKsqlConsumer<Vehicle> ksqlConsumer)
        {
            _ksqlConsumer = ksqlConsumer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _ksqlConsumer.Start(cancellationToken);

            await _ksqlConsumer.Subscription.ForEachAsync(Console.WriteLine, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}