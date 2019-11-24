using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using FluentValidation;
using Grpc.Core;

namespace DemoAndDiscourse.Logic.Services.Vehicle
{
    public class VehicleWriteService : VehicleWrite.VehicleWriteBase
    {
        private readonly KafkaProducer<string, Contracts.Vehicle> _vehicleProducer;
        private readonly IValidator<Contracts.Vehicle> _validator;

        public VehicleWriteService(KafkaProducer<string, Contracts.Vehicle> vehicleProducer, IValidator<Contracts.Vehicle> validator)
        {
            _vehicleProducer = vehicleProducer;
            _validator = validator;
        }

        public override async Task<Contracts.Vehicle> AddVehicle(Contracts.Vehicle request, ServerCallContext context)
        {
            await _validator.ValidateOrThrowAsync(request);
            await _vehicleProducer.ProduceAsync(request, request.Vin);
            return request;
        }
    }
}