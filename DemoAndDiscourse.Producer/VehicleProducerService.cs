using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.Utils;
using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.Producer
{
    public sealed class VehicleProducerService : IHostedService
    {
        private readonly KafkaProducer<Null, Vehicle> _producer;

        public VehicleProducerService(KafkaProducer<Null, Vehicle> producer)
        {
            _producer = producer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var randomVehicle = new Vehicle
                {
                    Vin = RandomUtils.RandomVin,
                    LocationCode = "PHX-IC"
                };
                await _producer.ProduceAsync(randomVehicle, null);
                Console.WriteLine(randomVehicle.ToString());

                await Task.Delay(5_000, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}