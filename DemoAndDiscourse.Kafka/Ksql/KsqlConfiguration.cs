using System;
using System.Threading;
using DemoAndDiscourse.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.Kafka.Ksql
{
    public static class KsqlConfiguration
    {
        public static IServiceCollection AddTableMapper(this IServiceCollection services)
        {
            services.AddSingleton(AutoMapperConfig.GetMapper());
            return services.AddSingleton<TableMapper>();
        }

        public static IServiceCollection AddKsqlClient(this IServiceCollection services, Uri ksqlServerHost)
        {
            services.AddHttpClient<KsqlClient>((provider, client) =>
            {
                client.BaseAddress = ksqlServerHost;
                client.Timeout = Timeout.InfiniteTimeSpan;
            });
            return services;
        }

        public static IServiceCollection AddKsqlConsumer<TRow>(this IServiceCollection services, KsqlQuery query)
        {
            return services.AddTransient(provider =>
                new KafkaKsqlConsumer<TRow>(
                    provider.GetService<KsqlClient>(),
                    query,
                    provider.GetService<TableMapper>()
                )
            );
        }
    }
}