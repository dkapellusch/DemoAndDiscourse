using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DemoAndDiscourse.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoAndDiscourse.Kafka.Ksql
{
    public sealed class KafkaKsqlConsumer<TRow>
    {
        private readonly KsqlClient _ksqlRestClient;
        private readonly KsqlQuery _query;
        private readonly TableMapper _mapper;
        private Subject<TRow> _streamSubject;

        public KafkaKsqlConsumer(KsqlClient ksqlRestClient, KsqlQuery query, TableMapper mapper)
        {
            _ksqlRestClient = ksqlRestClient;
            _query = query;
            _mapper = mapper;
        }

        public IObservable<TRow> Subscription => _streamSubject?.AsObservable();

        public void Start(CancellationToken token)
        {
            if (_streamSubject != null)
                return;

            _streamSubject = new Subject<TRow>();
            _ = Consume(token);
        }

        private async Task Consume(CancellationToken token)
        {
            await using var queryStream = await _ksqlRestClient.ExecuteQuery(_query, token);
            using var streamReader = new StreamReader(queryStream);

            while (!streamReader.EndOfStream && !token.IsCancellationRequested)
            {
                var line = await streamReader.ReadLineAsync();

                if (string.IsNullOrEmpty(line))
                    continue;

                var streamResponse = JsonConvert.DeserializeObject<StreamResponse>(line);

                if (streamResponse.Row?.Columns is null || !streamResponse.Row.Columns.Any())
                    break;

                var values = streamResponse.Row.Columns.Skip(2).SelectMany(FlattenNestedValues).ToArray();
                var payload = _mapper.Map<TRow>(values);

                _streamSubject.OnNext(payload);
            }
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