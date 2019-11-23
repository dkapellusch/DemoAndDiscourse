using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Logic;
using DemoAndDiscourse.Logic.Services.Location;

namespace DemoAndDiscourse.GraphqlGateway.Graphql.Vehicle
{
    public class VehicleLocationResolver : IResolver<Contracts.Vehicle, Contracts.Location>
    {
        private readonly LocationReadService _locationReadService;

        public VehicleLocationResolver(LocationReadService locationReadService)
        {
            _locationReadService = locationReadService;
        }

        public Task<Contracts.Location> Resolve(Contracts.Vehicle source) =>
            _locationReadService.GetLocation(new LocationRequest {LocationCode = source.LocationCode}, new InMemoryGrpcServerCallContext());
    }
}