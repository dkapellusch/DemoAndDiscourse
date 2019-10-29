using System;
using System.Linq;
using System.Reactive.Linq;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.Server
{
    public static class GrpcUtilities
    {
        public static IWebHostBuilder ConfigureGrpcServer(this IWebHostBuilder builder, params Action<IEndpointRouteBuilder>[] endPoints)
        {
            builder.ConfigureServices(services => services.AddGrpc());
            builder.Configure(appBuilder =>
            {
                appBuilder.UseRouting();

                appBuilder.UseEndpoints(endpointBuilder => { endPoints.ToList().ForEach(e => e(endpointBuilder)); });
            });

            return builder;
        }

        public static IObservable<T> AsObservable<T>(this IAsyncStreamReader<T> streamReader) where T : class
        {
            return Observable.FromAsync(async _ =>
                {
                    var hasNext = streamReader != null && await streamReader.MoveNext();
                    return hasNext ? streamReader.Current : null;
                })
                .Repeat()
                .TakeWhile(data => !(data is null));
        }
    }
}