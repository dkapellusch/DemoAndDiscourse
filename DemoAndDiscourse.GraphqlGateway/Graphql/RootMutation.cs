using DemoAndDiscourse.GraphqlGateway.Graphql.Location;
using DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle;
using GraphQL;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql
{
    public class RootMutation : ObjectGraphType
    {
        private readonly IDependencyResolver _resolver;

        public RootMutation(IDependencyResolver resolver)
        {
            _resolver = resolver;
            AddMutations<AddOrUpdateVehicle>();
            AddMutations<AddOrUpdateLocation>();
        }

        private void AddMutations<T>() where T : IComplexGraphType
        {
            var subQuery = _resolver.Resolve<T>();
            foreach (var field in subQuery.Fields)
            {
                Field(field.Type, field.Name, field.Description, field.Arguments, ctx => field.Resolver.Resolve(ctx as ResolveFieldContext));
            }
        }
    }
}