using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Mutations;

    public record UpdateProfileInput(int Id, string Firstname, string Lastname, string Mobile);
    public record ProfileMessage(User User, string Message);

    [ExtendObjectType("Mutation")]
    public class UpdateProfile
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
        [UseMutationConvention]
        public async Task<ProfileMessage> ProfileUpdate(
            UpdateProfileInput input, 
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

            user.Firstname = input.Firstname;
            user.Lastname = input.Lastname;
            user.Mobile = input.Mobile;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return new ProfileMessage(user, "You have updated your profile successfully.");
        }
    }

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation UpdateProfile($input: UpdateProfileInput!) {
//   profileUpdate(input: $input) {
//     message
//   }
// }

// GraphQL Variables
// {
//   "input": {
//     "id": 1,
//     "firstname": "Reynaldo",
//     "lastname": "Marquez-Gragasin",
//     "mobile": "+633433343"
//   }
// }
