using System;
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
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                    services
                        .AddTableMapper()
                        .AddKsqlClient(new Uri("http://localhost:8088/query"))
                        .AddKsqlConsumer<Vehicle>(new KsqlQuery
                        {
                            Ksql = "SELECT * from VEHICLES limit 1;",
                            StreamProperties = {{"auto.offset.reset", "earliest"}}
                        })
                        .AddHostedService<KsqlConsumerService>()
                )
                .Build()
                .RunAsync();
        }
    }
}