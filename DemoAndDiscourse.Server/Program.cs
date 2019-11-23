using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.Server
{
    internal class Program
    {
        public static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

        private static IWebHostBuilder CreateHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
            .ConfigureKestrel(options => options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http2))
            .ConfigureGrpcServer(
//                _ => _.MapGrpcService<VehicleService>(),
//                _ => _.MapGrpcService<LocationService>()
            )
            .ConfigureServices((hostContext, services) => services
                .AddKafkaConsumer<Vehicle>(new ConsumerConfig
                {
                    BootstrapServers = "localhost:39092",
                    ClientId = Guid.NewGuid().ToString(),
                    GroupId = Guid.NewGuid().ToString(),
                    EnableAutoCommit = false,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                })
                .AddKafkaConsumer<Location>(new ConsumerConfig
                {
                    BootstrapServers = "localhost:39092",
                    ClientId = Guid.NewGuid().ToString(),
                    GroupId = Guid.NewGuid().ToString(),
                    EnableAutoCommit = false,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                })
                .AddKafkaProducer<Null, Vehicle>(new ProducerConfig
                {
                    BootstrapServers = "localhost:39092",
                    ClientId = Guid.NewGuid().ToString()
                })
                .AddKafkaProducer<Null, Location>(new ProducerConfig
                {
                    BootstrapServers = "localhost:39092",
                    ClientId = Guid.NewGuid().ToString()
                })
                .AddSingleton(typeof(IMessageSerializer<>), typeof(JsonMessageSerializer<>))
            );
    }
}