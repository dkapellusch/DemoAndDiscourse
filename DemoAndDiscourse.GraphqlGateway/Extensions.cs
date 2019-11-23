using System;
using System.Linq;
using System.Reflection;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.GraphqlGateway.Graphql.Location;
using DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.Utils;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server.Transports.WebSockets;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using LocationReadService = DemoAndDiscourse.Logic.Services.Location.LocationReadService;
using LocationWriteService = DemoAndDiscourse.Logic.Services.Location.LocationWriteService;
using Schema = DemoAndDiscourse.GraphqlGateway.Graphql.Schema;
using VehicleReadService = DemoAndDiscourse.Logic.Services.Vehicle.VehicleReadService;
using VehicleWriteService = DemoAndDiscourse.Logic.Services.Vehicle.VehicleWriteService;

namespace DemoAndDiscourse.GraphqlGateway
{
    public static class Extensions
    {
        public static T UpdateObject<T>(this T destination, T source)
        {
            foreach (var property in typeof(T).GetProperties().Where(p => p.CanWrite))
            {
                var sourceValue = property.GetValue(source, null);
                if (sourceValue is null) continue;

                property.SetValue(destination, sourceValue, null);
            }

            return destination;
        }

        public static IServiceCollection AddGraphqlTypes(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            foreach (var type in currentAssembly.DefinedTypes.Where(t => typeof(IGraphType).IsAssignableFrom(t)))
            {
                services.AddSingleton(type.UnderlyingSystemType);
            }

            return services.AddSingleton<IDependencyResolver, DependencyResolver>()
                .AddSingleton<IDocumentExecuter, DocumentExecuter>()
                .AddSingleton<IDocumentWriter, DocumentWriter>()
                .AddSingleton<ISchema, Schema>()
                .AddSingleton<Schema>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services) =>
            services
                .AddSingleton<VehicleReadService>()
                .AddSingleton<VehicleWriteService>()
                .AddSingleton<LocationReadService>()
                .AddSingleton<LocationWriteService>();

        public static IServiceCollection AddResolvers(this IServiceCollection services) =>
            services
                .AddSingleton<IResolver<Vehicle, Location>, VehicleLocationResolver>()
                .AddSingleton<IResolver<Location, Vehicle[]>, LocationVehicleResolver>();

        public static IServiceCollection AddKafka(this IServiceCollection services, string url) =>
            services
                .AddKafkaProducer<string, Vehicle>(new ProducerConfig
                {
                    BootstrapServers = url,
                    ClientId = Guid.NewGuid().ToString()
                })
                .AddKafkaConsumer<Vehicle>(new ConsumerConfig
                {
                    BootstrapServers = url,
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    ClientId = Guid.NewGuid().ToString(),
                    GroupId = Guid.NewGuid().ToString()
                })
                .AddKafkaProducer<string, Location>(new ProducerConfig
                {
                    BootstrapServers = url,
                    ClientId = Guid.NewGuid().ToString()
                })
                .AddKafkaConsumer<Location>(new ConsumerConfig
                {
                    BootstrapServers = url,
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    ClientId = Guid.NewGuid().ToString(),
                    GroupId = Guid.NewGuid().ToString()
                })
                .AddSingleton(typeof(IMessageSerializer<>), typeof(JsonMessageSerializer<>));
    }
}