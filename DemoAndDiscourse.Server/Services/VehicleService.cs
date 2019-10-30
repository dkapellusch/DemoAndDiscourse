using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using Grpc.Core;

namespace DemoAndDiscourse.Server.Services
{
    public class VehicleService : Contracts.VehicleService.VehicleServiceBase
    {
        private readonly KafkaConsumer<Vehicle> _vehicleConsumer;
        private readonly KafkaProducer<Null, Vehicle> _vehicleProducer;

        public VehicleService(KafkaConsumer<Vehicle> vehicleConsumer, KafkaProducer<Null, Vehicle> vehicleProducer)
        {
            _vehicleConsumer = vehicleConsumer;
            _vehicleConsumer.Start();
            _vehicleProducer = vehicleProducer;
        }

        public override async Task<Vehicle> AddVehicle(Vehicle request, ServerCallContext context)
        {
            await _vehicleProducer.ProduceAsync(request, null);
            return request;
        }

        public override async Task<Vehicle> GetVehicle(VehicleRequest request, ServerCallContext context)
        {
            var vehicle = await _vehicleConsumer.Subscription.Timeout(TimeSpan.FromSeconds(10))
                .FirstOrDefaultAsync(v => v.Value.Vin == request.Vin)
                .Catch(Observable.Empty<ConsumeResult<string, Vehicle>>());

            if (vehicle?.Value is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No vehicle found with vin {request.Vin}"));

            return vehicle.Value;
        }

        public override async Task GetVehicleStream(Empty request, IServerStreamWriter<Vehicle> responseStream, ServerCallContext context) =>
            await _vehicleConsumer.Subscription.ForEachAsync(async v => await responseStream.WriteAsync(v.Value), context.CancellationToken);
    }
}