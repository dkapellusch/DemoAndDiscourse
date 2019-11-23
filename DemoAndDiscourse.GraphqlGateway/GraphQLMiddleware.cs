using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Server.Transports.AspNetCore.Common;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DemoAndDiscourse.GraphqlGateway
{
    public class GraphQLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;

        public GraphQLMiddleware
        (
            RequestDelegate next,
            IDocumentExecuter executer,
            IDocumentWriter writer)
        {
            _next = next;
            _executer = executer;
            _writer = writer;
        }

        public async Task Invoke(HttpContext context, ISchema schema)
        {
            if (!IsGraphQLRequest(context))
            {
                await _next(context);
                return;
            }

            await ExecuteAsync(context, schema);
        }

        private bool IsGraphQLRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/api/graphql")
                   && string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase);
        }

        private async Task ExecuteAsync(HttpContext context, ISchema schema)
        {
            var request = Deserialize<GraphQLRequest>(context.Request.Body);

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = request?.Query;
                _.OperationName = request?.OperationName;
                _.Inputs = request?.Variables.ToInputs();
                _.EnableMetrics = true;
                _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
            });

            await WriteResponseAsync(context, result);
        }

        private async Task WriteResponseAsync(HttpContext context, ExecutionResult result)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.Errors?.Any() == true ? (int) HttpStatusCode.BadRequest : (int) HttpStatusCode.OK;

            await _writer.WriteAsync(context.Response.Body, result);
        }

        public static T Deserialize<T>(Stream s)
        {
            using var reader = new StreamReader(s);
            using var jsonReader = new JsonTextReader(reader);
            var ser = new JsonSerializer();
            return ser.Deserialize<T>(jsonReader);
        }
    }
}