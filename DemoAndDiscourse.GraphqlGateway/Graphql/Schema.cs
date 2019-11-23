using DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle;
using GraphQL;

namespace DemoAndDiscourse.GraphqlGateway.Graphql
{
    public class Schema : GraphQL.Types.Schema
    {
        public Schema(IDependencyResolver resolver)
        {
            DependencyResolver = resolver;
            Query = resolver.Resolve<RootQuery>();
            Mutation = resolver.Resolve<RootMutation>();
            Subscription = resolver.Resolve<VehicleSubscription>();
        }
    }
}