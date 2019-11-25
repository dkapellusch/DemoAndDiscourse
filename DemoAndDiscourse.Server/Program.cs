using System.Threading.Tasks;
using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Location;
using DemoAndDiscourse.Logic.Services.Vehicle;
using DemoAndDiscourse.RocksDb.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DemoAndDiscourse.Server
{
    internal class Program
    {
        public static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

        private static IWebHostBuilder CreateHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
            .ConfigureKestrel(options => options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http2))
            .ConfigureGrpcServer(
                _ => _.MapGrpcService<VehicleReadService>(),
                _ => _.MapGrpcService<VehicleWriteService>(),
                _ => _.MapGrpcService<LocationReadService>(),
                _ => _.MapGrpcService<LocationWriteService>()
            )
            .ConfigureServices((hostContext, services) => services
                .AddServices()
                .AddValidators()
                .AddRocksDb("./Graph.db")
                .AddKafka("localhost:39092")
            );
    }
}