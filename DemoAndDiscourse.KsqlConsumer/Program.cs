using System;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka.Ksql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.KsqlConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                    services
                        .AddTableMapper()
                        .AddKsqlClient(new Uri("http://localhost:8088/query"))
                        .AddKsqlConsumer<Vehicle>(new KsqlQuery
                        {
                            Ksql = "select * from vehicles limit 1;",
                            StreamProperties = {{"auto.offset.reset", "earliest"}}
                        })
                )
                .Build();

            var kCOnsumer = host.Services.GetService<KafkaKsqlConsumer<Vehicle>>();
            kCOnsumer.Start(default);
            await kCOnsumer.Subscription.ForEachAsync(Console.WriteLine);
        }
    }
}