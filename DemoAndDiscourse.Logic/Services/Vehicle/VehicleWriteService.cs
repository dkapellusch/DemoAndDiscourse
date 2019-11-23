using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using Grpc.Core;

namespace DemoAndDiscourse.Logic.Services.Vehicle
{
    public class VehicleWriteService : VehicleWrite.VehicleWriteBase
    {
        private readonly KafkaProducer<string, Contracts.Vehicle> _vehicleProducer;

        public VehicleWriteService(KafkaProducer<string, Contracts.Vehicle> vehicleProducer)
        {
            _vehicleProducer = vehicleProducer;
        }

        public override async Task<Contracts.Vehicle> AddVehicle(Contracts.Vehicle request, ServerCallContext context)
        {
            await _vehicleProducer.ProduceAsync(request, request.Vin);
            return request;
        }
    }
}