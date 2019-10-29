using Confluent.Kafka;
using DemoAndDiscourse.Utils;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.Kafka
{
    public static class Configuration
    {
        public static IServiceCollection AddKafkaConsumer<TPayload>(this IServiceCollection services, ConsumerConfig config, string topic = null) where TPayload : IMessage<TPayload>, new()
        {
            return services.AddSingleton(p => new KafkaConsumer<TPayload>(config, p.GetService<IMessageSerializer<TPayload>>(), topic));
        }

        public static IServiceCollection AddKafkaProducer<TKey, TPayload>(this IServiceCollection services, ProducerConfig config, string topic = null) where TPayload : IMessage<TPayload>, new()
        {
            return services.AddSingleton(p => new KafkaProducer<TKey, TPayload>(config, p.GetService<IMessageSerializer<TPayload>>(), topic));
        }
    }
}