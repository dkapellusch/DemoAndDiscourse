using System.Collections.Generic;
using Newtonsoft.Json;

namespace DemoAndDiscourse.Kafka.Ksql
{
    public sealed class KsqlQuery
    {
        [JsonProperty("ksql")] public string Ksql { get; set; }

        [JsonProperty("streamsProperties")] public Dictionary<string, string> StreamProperties { get; } = new Dictionary<string, string>();
    }

    public sealed class StreamResponse
    {
        public Row Row { get; set; }
    }

    public sealed class Row
    {
        public object[] Columns { get; set; }
    }
}