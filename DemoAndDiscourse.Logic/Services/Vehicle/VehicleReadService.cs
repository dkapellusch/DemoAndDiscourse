using System.Reactive.Linq;
using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using Grpc.Core;

namespace DemoAndDiscourse.Logic.Services.Vehicle
{
    public class VehicleReadService : VehicleRead.VehicleReadBase
    {
        private readonly KafkaBackedDb<Contracts.Vehicle> _db;

        public VehicleReadService(KafkaBackedDb<Contracts.Vehicle> db)
        {
            _db = db;
        }

        public override Task<Contracts.Vehicle> GetVehicle(VehicleRequest request, ServerCallContext context)
        {
//            var result = _db.GetItem(request.Vin) ?? throw new RpcException(new Status(StatusCode.NotFound, "No vehicle found."));
            var result = _db.GetItem(request.Vin);
            return Task.FromResult(result);
        }

        public override Task<Vehicles> GetAllVehicles(Empty request, ServerCallContext context) => Task.FromResult(new Vehicles {Elements = {_db.GetAll()}});

        public override async Task GetVehicleUpdates(Empty request, IServerStreamWriter<Contracts.Vehicle> responseStream, ServerCallContext context)
        {
            await _db.GetChanges().ForEachAsync(async m => await responseStream.WriteAsync(m), context.CancellationToken);
        }
    }
}