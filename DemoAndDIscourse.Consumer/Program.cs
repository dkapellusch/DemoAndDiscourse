using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.Consumer
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                    services.AddKafkaConsumer<Vehicle>(new ConsumerConfig
                            {
                                BootstrapServers = "localhost:39092",
                                ClientId = Guid.NewGuid().ToString(),
                                GroupId = "InventoryConsumer",
                                EnableAutoCommit = false,
                                AutoOffsetReset = AutoOffsetReset.Earliest
                            },
                            "Vehicles")
                        .AddSingleton(typeof(IMessageSerializer<>), typeof(JsonMessageMessageSerializer<>))
                        .AddHostedService<VehicleConsumerService>()
                )
                .UseConsoleLifetime();
    }
}