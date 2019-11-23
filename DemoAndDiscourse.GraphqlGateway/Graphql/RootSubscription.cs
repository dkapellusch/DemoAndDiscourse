using DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle;
using GraphQL;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql
{
    public class RootSubscription : ObjectGraphType
    {
        private readonly IDependencyResolver _resolver;

        public RootSubscription(IDependencyResolver resolver)
        {
            _resolver = resolver;
            AddSubscriptions<VehicleSubscription>();
        }

        private void AddSubscriptions<T>() where T : IComplexGraphType
        {
            var subQuery = _resolver.Resolve<T>();
            foreach (var field in subQuery.Fields)
            {
                Field(field.Type, field.Name, field.Description, field.Arguments, ctx => field.Resolver.Resolve(ctx as ResolveFieldContext));
            }
        }
    }
}