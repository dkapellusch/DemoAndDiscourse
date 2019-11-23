using System.Linq;
using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Vehicle;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Location
{
    public class LocationVehicleResolver : IResolver<Contracts.Location, Contracts.Vehicle[]>
    {
        private readonly VehicleReadService _vehicleReadService;

        public LocationVehicleResolver(VehicleReadService vehicleReadService)
        {
            _vehicleReadService = vehicleReadService;
        }

        public async Task<Contracts.Vehicle[]> Resolve(Contracts.Location source)
        {
            var vehicles = await _vehicleReadService.GetAllVehicles(new Empty(), new InMemoryGrpcServerCallContext());
            return vehicles.Elements.Where(v => v.LocationCode == source.LocationCode).ToArray();
        }
    }
}