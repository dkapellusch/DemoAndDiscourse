﻿using System.IO;
using System.Threading.Tasks;
using DemoAndDiscourse.GraphqlGateway.Graphql;
using DemoAndDiscourse.Logic;
using DemoAndDiscourse.RocksDb.Extensions;
using GraphQL.Server;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.GraphqlGateway
{
    class Program
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();

        static Task Main(string[] args) => CreateWebHostBuilder(args).Build().RunAsync();

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(Configuration)
                .ConfigureServices(services => services
                    .AddRocksDb("./Graph.db")
                    .AddKafka("localhost:39092")
                    .AddSingleton(typeof(KafkaBackedDb<>))
                    .AddServices()
                    .AddValidators()
                    .AddResolvers()
                    .AddGraphqlTypes()
                    .AddHttpContextAccessor()
                    .AddGraphQL()
                    .AddWebSockets()
                    .AddDataLoader()
                )
                .Configure((context, builder) =>
                    builder
                        .UseWebSockets()
                        .UseGraphQLWebSockets<Schema>("/api/graphql")
                        .UseMiddleware<GraphQLMiddleware>()
                        .UseGraphQLPlayground(new GraphQLPlaygroundOptions {Path = "/api/graphql/playground", GraphQLEndPoint = "/api/graphql"})
                        .UseGraphQLVoyager(new GraphQLVoyagerOptions {Path = "/api/graphql/voyager", GraphQLEndPoint = "/api/graphql"})
                        .UseGraphiQLServer(new GraphiQLOptions {GraphiQLPath = "/api/graphql/graphiql", GraphQLEndPoint = "/api/graphql"})
                )
                .UseKestrel()
                .ConfigureKestrel(k => k.AllowSynchronousIO = true);
    }
}