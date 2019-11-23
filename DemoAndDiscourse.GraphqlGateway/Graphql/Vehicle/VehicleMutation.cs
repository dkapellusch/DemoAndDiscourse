using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Vehicle;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle
{
    public class VehicleMutation : ObjectGraphType
    {
        public VehicleMutation(VehicleWriteService vehicleService)
        {
            FieldAsync<VehicleType>(GetType().Name,
                "Add or update a vehicle",
                new QueryArguments(new QueryArgument<NonNullGraphType<VehicleInputType>> {Name = "vehicle"}),
                async ctx =>
                {
                    var inputVehicle = ctx.GetArgument<Contracts.Vehicle>("vehicle");
                    return await vehicleService.AddVehicle(inputVehicle, new InMemoryGrpcServerCallContext());
                });
        }
    }
}