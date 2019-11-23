using DemoAndDiscourse.RocksDb.RocksAbstractions;
using DemoAndDiscourse.RocksDb.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.RocksDb.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddRocksDb(this IServiceCollection services, string pathToDb) =>
            services.AddSingleton(new RocksDatabase(pathToDb))
                .AddSingleton<RocksStore>()
                .AddSingleton(typeof(RocksLog<>))
                .AddSingleton(typeof(RocksDictionary<,>))
                .AddSingleton<ISerializer, JsonSerializer>()
                .AddSingleton(typeof(ISerializer<>), typeof(JsonSerializer<>));
    }
}