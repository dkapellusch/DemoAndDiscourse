using System.Threading.Tasks;

namespace DemoAndDiscourse.GraphqlGateway.Graphql
{
    public interface IResolver<TSource, TDestination>
    {
        Task<TDestination> Resolve(TSource source);
    }
}