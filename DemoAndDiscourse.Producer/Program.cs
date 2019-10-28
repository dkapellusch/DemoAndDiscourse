using Microsoft.Extensions.Hosting;

namespace DemoAndDiscourse.Producer
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => { })
                .UseConsoleLifetime();
    }
}