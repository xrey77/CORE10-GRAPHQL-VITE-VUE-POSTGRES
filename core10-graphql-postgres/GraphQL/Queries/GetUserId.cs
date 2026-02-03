using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Queries;

[ExtendObjectType("Query")]
public class GetUser
{
    [UseProjection]
    public async Task<User> GetUserById(
        int id,
        [Service]GraphqlDbContext context)
    {
        var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);


        if (user is null) {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("User ID not found.")
                    .SetCode("USER ID_NOT_FOUND")
                    .Build());
        }
        return user;   
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

