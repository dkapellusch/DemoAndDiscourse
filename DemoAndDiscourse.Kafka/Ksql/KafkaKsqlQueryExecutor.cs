using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DemoAndDiscourse.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoAndDiscourse.Kafka.Ksql
{
    public class KafkaKsqlQueryExecutor
    {
        private readonly KsqlClient _ksqlRestClient;
        private readonly TableMapper _mapper;

        public KafkaKsqlQueryExecutor(KsqlClient ksqlRestClient, TableMapper mapper)
        {
            _ksqlRestClient = ksqlRestClient;
            _mapper = mapper;
        }

        public async Task<IEnumerable<T>> ExecuteQuery<T>(KsqlQuery query)
        {
            await using var queryStream = await _ksqlRestClient.ExecuteQueryAsync(query);
            using var streamReader = new StreamReader(queryStream);
            var results = new List<T>();

            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();

                if (string.IsNullOrEmpty(line))
                    continue;

                var streamResponse = JsonConvert.DeserializeObject<StreamResponse>(line);

                if (streamResponse.Row?.Columns is null || !streamResponse.Row.Columns.Any())
                    break;

                var values = streamResponse.Row.Columns.Skip(2).SelectMany(FlattenNestedValues).ToArray();
                var payload = _mapper.Map<T>(values);
                results.Add(payload);
            }

            return results;
        }

        private static List<string> FlattenNestedValues(object someObj)
        {
            if (!(someObj is JObject jObject))
                return new List<string> {someObj?.ToString() ?? string.Empty};

            var values = new List<string>();

            foreach (var property in jObject.Properties())
            {
                if (property.Value is JObject complexObject)
                    values.AddRange(FlattenNestedValues(complexObject));

                else if (property.HasValues)
                    values.Add(property.Value.Value<string>());
            }

            return values;
        }
    }
}