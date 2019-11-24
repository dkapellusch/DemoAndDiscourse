using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Vehicle;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle
{
    public class VehicleMutation : ObjectGraphType
    {
        public VehicleMutation(VehicleWriteService vehicleService, VehicleReadService readService)
        {
            FieldAsync<VehicleType>(GetType().Name,
                "Add or update a vehicle",
                new QueryArguments(new QueryArgument<NonNullGraphType<VehicleInputType>> {Name = "vehicle"}),
                async ctx => await ctx.TryAsyncResolve(async context =>
                {
                    var inputVehicle = ctx.GetArgument<Contracts.Vehicle>("vehicle");
                    var currentVehicle = await readService.GetVehicle(new VehicleRequest {Vin = inputVehicle.Vin}, new InMemoryGrpcServerCallContext());

                    if (currentVehicle != null)
                        inputVehicle = inputVehicle.UpdateObject(currentVehicle);

                    return await vehicleService.AddVehicle(inputVehicle, new InMemoryGrpcServerCallContext());
                })
            );
        }
    }
}