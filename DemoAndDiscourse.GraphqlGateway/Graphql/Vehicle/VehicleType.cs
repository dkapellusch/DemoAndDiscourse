using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle
{
    public class VehicleType : ObjectGraphType<Contracts.Vehicle>
    {
        public VehicleType(IResolver<Contracts.Vehicle, Contracts.Location> locationResolver)
        {
            Field<IdGraphType>(nameof(Contracts.Vehicle.Vin));
            Field(v => v.Make);
            Field(v => v.Model);
            Field<Location.LocationType>("location",
                "a location",
                resolve: ctx => locationResolver.Resolve(ctx.Source)
            );
        }
    }

    public class VehicleInputType : InputObjectGraphType<Contracts.Vehicle>
    {
        public VehicleInputType()
        {
            Field<IdGraphType>(nameof(Contracts.Vehicle.Vin));
            Field(v => v.Make, nullable: true);
            Field(v => v.Model, nullable: true);
            Field(v => v.LocationCode, nullable: true);
        }
    }
}