using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;

namespace core10_graphql_postgres.GraphQL.Queries;

[ExtendObjectType("Query")]
public class GetallUsers
{
    [UseProjection] // Enables EF to only fetch requested fields
    [UseFiltering]
    [UseSorting]    

    public IQueryable<User> ListallUsers(GraphqlDbContext context) 
        => context.Users;
}