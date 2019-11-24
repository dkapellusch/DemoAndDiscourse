using DemoAndDiscourse.Contracts;
using DemoAndDiscourse.Logic.Services.Location;
using FluentValidation;

namespace DemoAndDiscourse.Logic.Services.Vehicle
{
    public class VehicleValidator : AbstractValidator<Contracts.Vehicle>
    {
        public VehicleValidator(LocationReadService locationReadService)
        {
            RuleFor(v => v.Vin).NotNull().NotEmpty().Length(17);
            RuleFor(v => v.LocationCode)
                .MustAsync(async
                    (v, l, token) => string.IsNullOrEmpty(l) || await locationReadService.GetLocation(new LocationRequest {LocationCode = l}, new InMemoryGrpcServerCallContext()) != null)
                .WithMessage(v => $"No location was found for location code: {v.LocationCode}.");
        }
    }
}