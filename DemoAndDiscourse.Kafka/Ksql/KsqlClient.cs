using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DemoAndDiscourse.Kafka.Ksql
{
    public sealed class KsqlClient
    {
        private readonly HttpClient _client;

        public KsqlClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<Stream> ExecuteQuery(KsqlQuery query, CancellationToken token = default)
        {
            var request = JsonConvert.SerializeObject(query);
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "")
                {
                    Content = new StringContent(request, Encoding.UTF8, "application/vnd.ksql.v1+json"),
                    Headers = {{"accept", MediaTypeWithQualityHeaderValue.Parse("application/vnd.ksql.v1+json").ToString()}}
                },
                HttpCompletionOption.ResponseHeadersRead,
                token);

            return await response.Content.ReadAsStreamAsync();
        }
    }
}