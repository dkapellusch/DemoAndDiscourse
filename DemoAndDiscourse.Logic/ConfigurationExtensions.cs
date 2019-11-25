using System;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using DemoAndDiscourse.Logic.Services.Location;
using DemoAndDiscourse.Logic.Services.Vehicle;
using DemoAndDiscourse.Utils;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.Logic
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services) =>
            services
                .AddSingleton<VehicleReadService>()
                .AddSingleton<VehicleWriteService>()
                .AddSingleton<LocationReadService>()
                .AddSingleton<LocationWriteService>();

        public static IServiceCollection AddValidators(this IServiceCollection services) =>
            services
                .AddTransient<IValidator<Vehicle>, VehicleValidator>();

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