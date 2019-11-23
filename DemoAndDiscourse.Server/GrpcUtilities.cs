using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.Server
{
    public static class GrpcUtilities
    {
        public static IWebHostBuilder ConfigureGrpcServer(this IWebHostBuilder builder, params Action<IEndpointRouteBuilder>[] endPoints) =>
            builder.ConfigureServices(services => services.AddGrpc())
                .Configure(appBuilder => appBuilder.UseRouting()
                    .UseEndpoints(endpointBuilder => endPoints.ToList()
                        .ForEach(e => e(endpointBuilder))
                    )
                );
    }
}