using System.Threading.Tasks;

namespace DemoAndDiscourse.GraphqlGateway
{
    public interface IResolver<TSource, TDestination>
    {
        Task<TDestination> Resolve(TSource source);
    }
}