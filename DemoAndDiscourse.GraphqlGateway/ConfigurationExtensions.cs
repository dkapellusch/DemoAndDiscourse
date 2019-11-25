using System.Linq;
using System.Reflection;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.GraphqlGateway.Graphql;
using DemoAndDiscourse.GraphqlGateway.Graphql.Location;
using DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Schema = DemoAndDiscourse.GraphqlGateway.Graphql.Schema;

namespace DemoAndDiscourse.GraphqlGateway
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddGraphqlTypes(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            foreach (var type in currentAssembly.DefinedTypes.Where(t => typeof(IGraphType).IsAssignableFrom(t)))
            {
                services.AddSingleton(type.UnderlyingSystemType);
            }

            return services
                .AddSingleton<IDependencyResolver, DependencyResolver>()
                .AddSingleton<IDocumentExecuter, DocumentExecuter>()
                .AddSingleton<IDocumentWriter, DocumentWriter>()
                .AddSingleton<ISchema, Schema>()
                .AddSingleton<Schema>();
        }


        public static IServiceCollection AddResolvers(this IServiceCollection services) =>
            services
                .AddSingleton<IResolver<Vehicle, Location>, VehicleLocationResolver>()
                .AddSingleton<IResolver<Location, Vehicle[]>, LocationVehicleResolver>();
    }
}