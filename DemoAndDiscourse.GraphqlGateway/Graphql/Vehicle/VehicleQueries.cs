using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Logic;
using GraphQL.Types;
using VehicleReadService = DemoAndDiscourse.Logic.Services.Vehicle.VehicleReadService;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle
{
    public class VehicleQueries : ObjectGraphType<VehicleType>
    {
        public VehicleQueries(VehicleReadService vehicleService)
        {
            Name = "Vehicle";

            FieldAsync<VehicleType>("vehicle",
                "a vehicle",
                new QueryArguments(new QueryArgument(typeof(StringGraphType)) {Name = "vin"}),
                async ctx => await vehicleService.GetVehicle(new VehicleRequest {Vin = ctx.Arguments["vin"].ToString()}, null));

            FieldAsync<ListGraphType<VehicleType>>("vehiclesByPartialVin",
                "vehicles with partial vin",
                new QueryArguments(new QueryArgument(typeof(StringGraphType)) {Name = "vin"}),
                async ctx =>
                {
                    var vehicles = await vehicleService.GetVehiclesByPartialVin(new VehicleRequest {Vin = ctx.Arguments["vin"].ToString()}, null);
                    return vehicles.Elements;
                });

            FieldAsync<ListGraphType<VehicleType>>("vehicles",
                "all vehicles",
                null,
                async ctx =>
                {
                    var response = await vehicleService.GetAllVehicles(new Empty(), new InMemoryGrpcServerCallContext());
                    return response.Elements;
                });
        }
    }
}