using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using Grpc.Core;

namespace DemoAndDiscourse.Logic.Services.Location
{
    public class LocationReadService : LocationRead.LocationReadBase
    {
        private readonly KafkaBackedDb<Contracts.Location> _db;

        public LocationReadService(KafkaBackedDb<Contracts.Location> db)
        {
            _db = db;
        }

        public override Task<Contracts.Location> GetLocation(LocationRequest request, ServerCallContext context) =>
            Task.FromResult(_db.GetItem(request.LocationCode));

        public override Task<Locations> GetAllLocations(Empty request, ServerCallContext context) => Task.FromResult(new Locations {Elements = {_db.GetAll()}});
    }
}