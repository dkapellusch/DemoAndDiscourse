using System.Collections.Generic;
using Newtonsoft.Json;

namespace DemoAndDiscourse.Kafka.Ksql
{
    public class KsqlQuery
    {
        [JsonProperty("ksql")] public string Ksql { get; set; }

        [JsonProperty("streamsProperties")] public Dictionary<string, string> StreamProperties { get; } = new Dictionary<string, string>();
    }

    public abstract class StreamResponse
    {
        public Row Row { get; set; }
    }

    public abstract class Row
    {
        public object[] Columns { get; set; }
    }
}