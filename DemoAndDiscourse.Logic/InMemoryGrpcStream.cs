using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace DemoAndDiscourse.Logic
{
    public class InMemoryGrpcStream<T> : IServerStreamWriter<T>, IAsyncStreamReader<T>
    {
        private readonly Subject<T> _stream;

        public InMemoryGrpcStream()
        {
            _stream = new Subject<T>();
        }

        public Task WriteAsync(T message)
        {
            _stream.OnNext(message);
            return Task.CompletedTask;
        }

        public WriteOptions WriteOptions { get; set; } = new WriteOptions();

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            var next = _stream.Next().FirstOrDefault();
            var nextExists = !(next is null);

            if (nextExists) Current = next;

            return Task.FromResult(nextExists);
        }

        public T Current { get; private set; }
    }
}