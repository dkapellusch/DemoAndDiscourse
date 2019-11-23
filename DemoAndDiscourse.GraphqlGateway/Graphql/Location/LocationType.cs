using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Location
{
    public class LocationType : ObjectGraphType<Contracts.Location>
    {
        public LocationType(IResolver<Contracts.Location, Contracts.Vehicle[]> vehicleResolver)
        {
            Field(l => l.LocationId);
            Field(l => l.LocationCode);
            Field(l => l.LocationName);
            Field(typeof(ListGraphType<Vehicle.VehicleType>),
                "vehicles",
                "a location",
                resolve: ctx => vehicleResolver.Resolve(ctx.Source));
        }
    }

    public class LocationInputType : InputObjectGraphType<Contracts.Location>
    {
        public LocationInputType()
        {
            Field(l => l.LocationId);
            Field(l => l.LocationCode);
            Field(l => l.LocationName);
        }
    }
}