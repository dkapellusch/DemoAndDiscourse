using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace DemoAndDiscourse.Logic
{
    public class InMemoryGrpcServerCallContext : ServerCallContext
    {
        private Metadata _responseTrailersCore;

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
        {
            _responseTrailersCore = responseHeaders;
            return Task.CompletedTask;
        }

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options) => null;

        protected override string MethodCore { get; } = Guid.NewGuid().ToString();

        protected override string HostCore { get; } = Guid.NewGuid().ToString();

        protected override string PeerCore { get; } = Guid.NewGuid().ToString();

        protected override DateTime DeadlineCore { get; } = DateTime.UtcNow.AddMinutes(5);

        protected override Metadata RequestHeadersCore { get; } = new Metadata();

        protected override CancellationToken CancellationTokenCore { get; } = default;

        protected override Metadata ResponseTrailersCore => _responseTrailersCore;

        protected override Status StatusCore { get; set; }

        protected override WriteOptions WriteOptionsCore { get; set; }

        protected override AuthContext AuthContextCore { get; } = new AuthContext(Guid.NewGuid().ToString(), new Dictionary<string, List<AuthProperty>>());
    }
}