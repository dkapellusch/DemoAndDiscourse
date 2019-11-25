using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Location
{
    public class LocationType : ObjectGraphType<Contracts.Location>
    {
        public LocationType(IResolver<Contracts.Location, Contracts.Vehicle[]> vehicleResolver)
        {
            Field<IdGraphType>(nameof(Contracts.Location.LocationCode));
            Field(l => l.LocationName);
            Field<ListGraphType<Vehicle.VehicleType>>(
                "vehicles",
                "a location",
                resolve: ctx => vehicleResolver.Resolve(ctx.Source)
            );
        }
    }

    public class LocationInputType : InputObjectGraphType<Contracts.Location>
    {
        public LocationInputType()
        {
            Field<IdGraphType>(nameof(Contracts.Location.LocationCode));
            Field(l => l.LocationName, nullable: true);
        }
    }
}