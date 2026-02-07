using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Queries;

[ExtendObjectType("Query")]
public class GetUser
{
  [Authorize]    
  [UseProjection]
  public IQueryable<User> GetUserById(
        [Service] GraphqlDbContext context,
        int id)
    {
        return context.Users
            .AsNoTracking()
            .Where(u => u.Id == id);
    }
}

// ======Nitro Request======
// query GetUser($userid: Int!) {
//   userById(id: $userid) {
//     id
//     username
//     email
//   }
// }

// GraphQL Variables
// {
//   "userid": 1
// }

