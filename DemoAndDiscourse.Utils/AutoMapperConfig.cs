using AutoMapper;
using DemoAndDiscourse.Contracts;

namespace DemoAndDiscourse.Utils
{
    public sealed class TableMapper
    {
        private readonly IMapper _mapper;

        public TableMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TPayload Map<TPayload>(string[] columns) => _mapper.Map<TPayload>(columns);
    }

    public static class AutoMapperConfig
    {
        public static IMapper GetMapper() =>
            new MapperConfiguration(c =>
                {
                    c.CreateMap<string[], Vehicle>()
                        .ForMember(v => v.Vin, opts => opts.MapFrom(s => s[0]))
                        .ForMember(v => v.Make, opts => opts.MapFrom(s => s[1]))
                        .ForMember(v => v.Model, opts => opts.MapFrom(s => s[2]))
                        .ForMember(v => v.LocationCode, opts => opts.MapFrom(s => s[3]));

                    c.CreateMap<string[], Location>()
                        .ForMember(l => l.LocationId, opts => opts.MapFrom(s => s[0]))
                        .ForMember(l => l.LocationName, opts => opts.MapFrom(s => s[1]))
                        .ForMember(l => l.LocationCode, opts => opts.MapFrom(s => s[2]));
                }
            ).CreateMapper();
    }
}