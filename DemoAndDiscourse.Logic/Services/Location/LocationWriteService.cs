using System.Threading.Tasks;
using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Kafka;
using Grpc.Core;

namespace DemoAndDiscourse.Logic.Services.Location
{
    public class LocationWriteService : LocationWrite.LocationWriteBase
    {
        private readonly KafkaProducer<string, Contracts.Location> _kafkaProducer;

        public LocationWriteService(KafkaProducer<string, Contracts.Location> kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        public override async Task<Contracts.Location> AddLocation(Contracts.Location request, ServerCallContext context)
        {
            await _kafkaProducer.ProduceAsync(request, request.LocationCode);
            return request;
        }
    }
}