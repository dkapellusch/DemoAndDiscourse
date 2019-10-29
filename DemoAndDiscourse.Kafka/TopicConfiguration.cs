using System.Collections.Generic;
using System.Globalization;

namespace DemoAndDiscourse.Kafka
{
    public sealed class TopicConfiguration
    {
        public string CleanUpPolicy { get; set; } = Kafka.CleanUpPolicy.Compact;

        public string CompressionType { get; set; } = Kafka.CompressionType.Default;

        public string MessageTimestampType { get; set; } = Kafka.MessageTimestampType.Default;

        public long DeleteRetentionMilliseconds { get; set; } = 86_400_000; // 1 day

        public long FileDeleteDelayMilliseconds { get; set; } = 60_000; // 1 minute

        public long FlushMessageIntervalMilliseconds { get; set; } = 9_223_372_036_854_775_807;

        public long FlushDataIntervalMillisconds { get; set; } = 9_223_372_036_854_775_807;

        public long MessageTimestampDifferenceMaxMilliseconds { get; set; } = 9_223_372_036_854_775_807;

        public string FollowerReplicationThrottledReplicas { get; set; } = string.Empty;

        public int IndexIntervalBytes { get; set; } = 4_096;

        public string LeaderReplicationThrottledReplicas { get; set; } = string.Empty;

        public int MaxMessageBytes { get; set; } = 1_000_012;

        public double MinCleanableDirtyRatio { get; set; } = 0.5;

        public long MinCompactionLagMilliseconds { get; set; } = 0;

        public int MinInSyncReplicas { get; set; } = 1;

        public bool PreAllocate { get; set; } = false;

        public long RetentionBytes { get; set; } = -1;

        public long RetentionMilliseconds { get; set; } = 604_800_000; // 7 days

        public int SegmentBytes { get; set; } = 1_073_741_824;

        public int SegmentIndexBytes { get; set; } = 10_485_760;

        public long SegmentJitterMilliseconds { get; set; } = 0;

        public long SegmentMilliseconds { get; set; } = 604_800_000; // 7 days

        public bool UncleanLeaderElectionEnable { get; set; } = false;

        public bool MessageDownConversionEnable { get; set; } = true;

        public Dictionary<string, string> GetConfigs() => new Dictionary<string, string>
        {
            {"cleanup.policy", CleanUpPolicy},
            {"compression.type", CompressionType},
            {"delete.retention.ms", DeleteRetentionMilliseconds.ToString()},
            {"file.delete.delay.ms", FileDeleteDelayMilliseconds.ToString()},
            {"flush.messages", FlushMessageIntervalMilliseconds.ToString()},
            {"flush.ms", FlushDataIntervalMillisconds.ToString()},
            {"follower.replication.throttled.replicas", FollowerReplicationThrottledReplicas},
            {"index.interval.bytes", IndexIntervalBytes.ToString()},
            {"leader.replication.throttled.replicas", LeaderReplicationThrottledReplicas},
            {"max.message.bytes", MaxMessageBytes.ToString()},
            {"message.timestamp.difference.max.ms", MessageTimestampDifferenceMaxMilliseconds.ToString()},
            {"message.timestamp.type", MessageTimestampType},
            {"min.cleanable.dirty.ratio", MinCleanableDirtyRatio.ToString(CultureInfo.InvariantCulture)},
            {"min.compaction.lag.ms", MinCompactionLagMilliseconds.ToString()},
            {"min.insync.replicas", MinInSyncReplicas.ToString()},
            {"preallocate", PreAllocate.ToString()},
            {"retention.bytes", RetentionBytes.ToString()},
            {"retention.ms", RetentionMilliseconds.ToString()},
            {"segment.bytes", SegmentBytes.ToString()},
            {"segment.index.bytes", SegmentIndexBytes.ToString()},
            {"segment.jitter.ms", SegmentJitterMilliseconds.ToString()},
            {"segment.ms", SegmentMilliseconds.ToString()},
            {"unclean.leader.election.enable", UncleanLeaderElectionEnable.ToString()},
            {"message.downconversion.enable", MessageDownConversionEnable.ToString()}
        };
    }

    public static class CleanUpPolicy
    {
        public static readonly string Delete = "delete";
        public static readonly string Compact = "compact";
        public static readonly string Default = Delete;
    }

    public static class CompressionType
    {
        public static readonly string Uncompressed = "uncompressed";
        public static readonly string Zstd = "zstd";
        public static readonly string Lz4 = "lz4";
        public static readonly string Snappy = "snappy";
        public static readonly string Gzip = "gzip";
        public static readonly string Producer = "producer";
        public static readonly string Default = Producer;
    }

    public static class MessageTimestampType
    {
        public static readonly string CreateTime = "CreateTime";
        public static readonly string LogAppendTime = "LogAppendTime";
        public static readonly string Default = CreateTime;
    }
}