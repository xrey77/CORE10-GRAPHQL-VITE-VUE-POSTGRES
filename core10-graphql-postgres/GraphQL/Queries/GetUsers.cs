using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate.Authorization;

namespace core10_graphql_postgres.GraphQL.Queries;

[ExtendObjectType("Query")]
public class GetallUsers
{
    [Authorize]
    [UseProjection]
    [UseFiltering]
    [UseSorting]    

    public IQueryable<User> ListallUsers(
        GraphqlDbContext context) 
        => context.Users;
}

// ======Nitro Request======
// query {
//   listallUsers {
//     id        
//     firstname
//     lastname
//     email
//     mobile
//     username
//     isactivated
//     isblocked
//     profilepic
//   }
// }

