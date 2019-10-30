using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace DemoAndDiscourse.Kafka
{
    public class KafkaAdmin
    {
        private readonly IAdminClient _innerClient;

        public KafkaAdmin(AdminClientConfig config)
        {
            _innerClient = new AdminClientBuilder(config).Build();
        }

        public Metadata GetMetadata(string topic) => _innerClient.GetMetadata(topic, TimeSpan.FromSeconds(30));

        public async Task CreateTopicAsync(string topic, TopicConfiguration topicConfiguration)
        {
            var topics = _innerClient.GetMetadata(TimeSpan.FromSeconds(30)).Topics;

            if (topics.Select(t => t.Topic).Contains(topic)) return;

            var initialTopics = new List<TopicSpecification>
            {
                new TopicSpecification
                {
                    Name = topic,
                    Configs = topicConfiguration.GetConfigs(),
                    NumPartitions = 25,
                    ReplicationFactor = 2
                }
            };

            await _innerClient.CreateTopicsAsync(initialTopics);
            await Task.Delay(1000);
        }

        public async Task ChangePartitionCountAsync(string topic, int partitionCount) =>
            await _innerClient.CreatePartitionsAsync(new[] {new PartitionsSpecification {Topic = topic, IncreaseTo = partitionCount}});


        public Metadata GetMetadata(TimeSpan? timeout = null) => _innerClient.GetMetadata(timeout ?? TimeSpan.FromMinutes(1));

        public async Task DeleteTopicAsync(string topicName)
        {
            var topics = _innerClient.GetMetadata(TimeSpan.FromSeconds(30)).Topics;

            if (topics.Select(t => t.Topic).Contains(topicName)) await _innerClient.DeleteTopicsAsync(new[] {topicName});

            await Task.Delay(5000);
        }
    }
}