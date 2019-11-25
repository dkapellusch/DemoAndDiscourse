using GraphQL.Types;

namespace DemoAndDiscourse.GraphqlGateway.Graphql
{
    public class MutationResult
    {
        public string Id { get; set; }
    }

    public class MutationResultType : ObjectGraphType<MutationResult>
    {
        public MutationResultType()
        {
            Field<IdGraphType>(nameof(MutationResult.Id), "The Id of the object effected by the mutation.");
        }
    }
}