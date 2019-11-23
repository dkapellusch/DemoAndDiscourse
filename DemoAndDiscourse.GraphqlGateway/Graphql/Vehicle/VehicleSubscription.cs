using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Vehicle;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle
{
    public sealed class VehicleSubscription : ObjectGraphType<Contracts.Vehicle>
    {
        private readonly VehicleReadService _vehicleReadService;

        public VehicleSubscription(VehicleReadService vehicleReadService)
        {
            _vehicleReadService = vehicleReadService;

            AddField(new EventStreamFieldType
            {
                Name = "vehicleChange",
                Type = typeof(VehicleType),
                Resolver = new FuncFieldResolver<Contracts.Vehicle>(ctx => ctx.Source as Contracts.Vehicle),
                Subscriber = new EventStreamResolver<Contracts.Vehicle>(GetVehicleChangeSubscription)
            });
        }

        private IObservable<Contracts.Vehicle> GetVehicleChangeSubscription(ResolveEventStreamContext context)
        {
            var outputStream = new InMemoryGrpcStream<Contracts.Vehicle>();
            _ = _vehicleReadService.GetVehicleUpdates(new Empty(), outputStream, new InMemoryGrpcServerCallContext());
            return outputStream.AsObservable();
        }
    }
}