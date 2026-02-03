using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Mutations;

    public record UpdatePasswordInput(int Id, string Password);
    public record ResponseMessage(User User, string Message);

    [ExtendObjectType("Mutation")]
    public class ChangeUserPassword
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
        [UseMutationConvention]
        public async Task<ResponseMessage> UpdatePassword(
            UpdatePasswordInput input, 
            [Service] GraphqlDbContext context)        
        {

            var user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == input.Id);

            if (user is null) {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("User ID not found.")
                        .SetCode("USER ID_NOT_FOUND")

                        .Build());
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);
            user.Password = hashedPassword;

            context.Users.Update(user);
            await context.SaveChangesAsync();
            return new ResponseMessage(user, "You have change your password successfully.");
        }
    }

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation UpdateUserPassword($input: UpdatePasswordInput!) {
//   updatePassword(input: $input) {
//     message
//   }
// }

// GraphQL Variables
// {
// {
//   "input": {
//     "id": 1,
//     "password": "nald"
//   }
// }
