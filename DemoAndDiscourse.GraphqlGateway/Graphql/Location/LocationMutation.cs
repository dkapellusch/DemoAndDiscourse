using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Location;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Location
{
    public class LocationMutation : ObjectGraphType
    {
        public LocationMutation(LocationWriteService locationService)
        {
            FieldAsync<LocationType>(GetType().Name,
                "Add or update a location",
                new QueryArguments(new QueryArgument<NonNullGraphType<LocationInputType>> {Name = "location"}),
                async ctx =>
                {
                    var inputLocation = ctx.GetArgument<Contracts.Location>("location");
                    var addedLocation = await locationService.AddLocation(inputLocation, new InMemoryGrpcServerCallContext());

                    return addedLocation;
                });
        }
    }
}