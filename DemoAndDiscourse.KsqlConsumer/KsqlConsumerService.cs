using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DemoAndDiscourse.Kafka.Ksql;
using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.KsqlConsumer
{
    public sealed class KsqlConsumerService<T> : IHostedService
    {
        private readonly KafkaKsqlConsumer<T> _ksqlConsumer;

        public KsqlConsumerService(KafkaKsqlConsumer<T> ksqlConsumer)
        {
            _ksqlConsumer = ksqlConsumer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _ksqlConsumer.Start(cancellationToken);

            await _ksqlConsumer.Subscription.ForEachAsync(m => Console.WriteLine(m), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}