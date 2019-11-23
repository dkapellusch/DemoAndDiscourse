using System;
using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka.Ksql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.KsqlConsumer
{
    internal static class Program
    {
        private static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                    services
                        .AddTableMapper()
                        .AddKsqlClient(new Uri("http://localhost:8088/query"))
                        .AddKsqlConsumer<Vehicle>(new KsqlQuery
                        {
                            Ksql = "SELECT * FROM VEHICLES LIMIT 1;",
                            StreamProperties = {{"auto.offset.reset", "earliest"}}
                        })
                        .AddHostedService<KsqlConsumerService<Vehicle>>()
                );
    }
}