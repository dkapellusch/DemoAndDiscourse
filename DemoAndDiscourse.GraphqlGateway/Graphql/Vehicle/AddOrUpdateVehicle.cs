using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Vehicle;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle
{
    public class AddOrUpdateVehicle : ObjectGraphType
    {
        public AddOrUpdateVehicle(VehicleWriteService vehicleService)
        {
            FieldAsync<MutationResultType>(GetType().Name,
                "Add or update a vehicle",
                new QueryArguments(new QueryArgument<NonNullGraphType<VehicleInputType>> {Name = "vehicle"}),
                async ctx => await ctx.TryAsyncResolve(async context =>
                {
                    var inputVehicle = ctx.GetArgument<Contracts.Vehicle>("vehicle");
                    var changedVehicle = await vehicleService.AddVehicle(inputVehicle, new InMemoryGrpcServerCallContext());
                    return new MutationResult {Id = changedVehicle.Vin};
                })
            );
        }
    }
}