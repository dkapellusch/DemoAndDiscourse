using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.GraphqlGateway.Graphql.Location;
using DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.Logic.Services.Vehicle;
using DemoAndDiscourse.Utils;
using FluentValidation;
using GraphQL;
using GraphQL.Http;
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
        public static async Task<T> TryHandleRequest<T>(this ResolveFieldContext<T> context, Func<ResolveFieldContext<T>, Task<T>> resolver)
        {
            try
            {
                return await resolver(context);
            }
            catch (Exception exception)
            {
                throw new ExecutionError(exception.Message);
            }
        }

        public static T UpdateObject<T>(this T destination, T source)
        {
            foreach (var property in typeof(T).GetProperties().Where(p => p.CanWrite))
            {
                var sourceValue = property.GetValue(source, null);
                var destinationValue = property.GetValue(destination, null);

                if (sourceValue is null || destinationValue != null && !string.IsNullOrEmpty(destinationValue.ToString())) continue;

                property.SetValue(destination, sourceValue, null);
            }

            return destination;
        }

        public static T UpdateObject<T>(this T current, IDictionary<string, object> updates)
        {
            var lowerCaseKeys = updates.Keys.Select(k => k.ToLowerInvariant()).ToHashSet();
            foreach (var property in typeof(T).GetProperties().Where(p => p.CanWrite && lowerCaseKeys.Contains(p.Name.ToLowerInvariant())))
            {
                var currentValue = property.GetValue(current, null);
                property.SetValue(updates[property.Name.ToLowerInvariant()], currentValue, null);
            }

            return current;
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

        public static IServiceCollection AddValidators(this IServiceCollection services) =>
            services
                .AddTransient<IValidator<Vehicle>, VehicleValidator>();

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
                    GroupId = "Gateway",
                    EnableAutoCommit = false
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
                    GroupId = "Gateway",
                    EnableAutoCommit = false
                })
                .AddSingleton(typeof(IMessageSerializer<>), typeof(JsonMessageSerializer<>));
    }
}