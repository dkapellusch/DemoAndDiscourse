using Confluent.Kafka;
using DemoAndDiscourse.Utils;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.Kafka
{
    public static class Configuration
    {
        public static IServiceCollection AddKafkaConsumer<TPayload>(this IServiceCollection services, ConsumerConfig config, string topic) where TPayload : IMessage<TPayload>, new()
        {
            services.AddSingleton(config);
            return services.AddSingleton(p => new KafkaConsumer<TPayload>(config, topic, p.GetService<IMessageSerializer<TPayload>>()));
        }

        public static IServiceCollection AddKafkaProducer<TKey, TPayload>(this IServiceCollection services, ProducerConfig config, string topic) where TPayload : IMessage<TPayload>, new()
        {
            services.AddSingleton(config);
            return services.AddSingleton(p => new KafkaProducer<TKey, TPayload>(config, topic, p.GetService<IMessageSerializer<TPayload>>()));
        }
    }
}