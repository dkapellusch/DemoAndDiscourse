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
        public static IMapper GetMapper()
        {
            return new MapperConfiguration(c =>
                {
                    c.CreateMap<string[], Vehicle>()
                        .ForMember(p => p.Vin, opts => opts.MapFrom(s => s[0]))
                        .ForMember(p => p.Make, opts => opts.MapFrom(s => s[1]))
                        .ForMember(p => p.Model, opts => opts.MapFrom(s => s[2]))
                        .ForMember(p => p.Year, opts => opts.MapFrom(s => s[3]))
                        .ForMember(p => p.PurchaseDate, opts => opts.MapFrom(s => s[4]));
                }
            ).CreateMapper();
        }
    }
}