using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using Grpc.Core;

namespace DemoAndDiscourse.Server.Services
{
    public class LocationService : Contracts.LocationService.LocationServiceBase
    {
        private readonly KafkaConsumer<Location> _locationConsumer;
        private readonly KafkaProducer<Null, Location> _locationProducer;

        public LocationService(KafkaConsumer<Location> locationConsumer, KafkaProducer<Null, Location> locationProducer)
        {
            _locationConsumer = locationConsumer;
            _locationConsumer.Start();
            _locationProducer = locationProducer;
        }

        public override async Task<Location> AddLocation(Location request, ServerCallContext context)
        {
            await _locationProducer.ProduceAsync(request, null);
            return request;
        }

        public override async Task<Location> GetLocation(LocationRequest request, ServerCallContext context)
        {
            var location = await _locationConsumer.Subscription.Timeout(TimeSpan.FromSeconds(10))
                .FirstOrDefaultAsync(l => l.Value.LocationId == request.LocationId || l.Value.LocationName == request.LocationName)
                .Catch(Observable.Empty<ConsumeResult<string, Location>>());

            if (location?.Value is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No location found for request {request}"));

            return location.Value;
        }
    }
}