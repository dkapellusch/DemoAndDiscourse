using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Location;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Location
{
    public class AddOrUpdateLocation : ObjectGraphType
    {
        public AddOrUpdateLocation(LocationWriteService locationService)
        {
            FieldAsync<MutationResultType>(GetType().Name,
                "Add or update a location",
                new QueryArguments(new QueryArgument<NonNullGraphType<LocationInputType>> {Name = "location"}),
                async ctx =>
                {
                    var inputLocation = ctx.GetArgument<Contracts.Location>("location");
                    var changedLocation = await locationService.AddLocation(inputLocation, new InMemoryGrpcServerCallContext());

                    return new MutationResult {Id = changedLocation.LocationCode};
                });
        }
    }
}