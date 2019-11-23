using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Logic;
using GraphQL.Types;
using LocationReadService = DemoAndDiscourse.Logic.Services.Location.LocationReadService;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Location
{
    public class LocationQueries : ObjectGraphType
    {
        public LocationQueries(LocationReadService locationService)
        {
            FieldAsync<LocationType>("location",
                "a location",
                new QueryArguments(new QueryArgument(typeof(StringGraphType)) {Name = "code"}),
                async ctx => await locationService.GetLocation(new LocationRequest {LocationCode = ctx.Arguments["code"].ToString()}, new InMemoryGrpcServerCallContext()));

            FieldAsync<ListGraphType<LocationType>>("locations",
                "all locations",
                null,
                async ctx =>
                {
                    var response = await locationService.GetAllLocations(new Empty(), new InMemoryGrpcServerCallContext());

                    return response.Elements;
                });
        }
    }
}