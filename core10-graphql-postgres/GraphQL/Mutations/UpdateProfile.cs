    using core10_graphql_postgres.Entities;
    using core10_graphql_postgres.Helpers;
    using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

    namespace core10_graphql_postgres.GraphQL.Mutations;

    public record ProfileInput(
        [property: ID] int Id,
        string Firstname,
        string Lastname,
        string Mobile
    );

    public record ProfileMessage(
        User User,
        string Message
    );

    [ExtendObjectType("Mutation")]
    public class UpdateUserProfile
    {
        [Authorize]
        [UseMutationConvention]
        public async Task<ProfileMessage> UpdateProfileAsync(
            ProfileInput input,
            GraphqlDbContext context) 
        {
            var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Id == input.Id);

            if (user is null) {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("User ID not found.")
                        .SetCode("USER_NOT_FOUND")
                        .Build());
            }

            user.Firstname = input.Firstname;
            user.Lastname = input.Lastname;
            user.Mobile = input.Mobile;

            await context.SaveChangesAsync();
            
            return new ProfileMessage(user, "You have updated your profile successfully.");
        }
    }

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation UpdateUserProfile($input: UpdateProfileInput!) {
//   updateProfile(input: $input) {
//         profileMessage {
//             user
//             {
//                 id
//                 firstname
//                 lastname
//                 mobile
//             }
//             message        
//         }
//   }
// }



// GraphQL Variables
// {
//   "input": {
//     "input": {
//     "id": 1, 
//     "firstname": "Rey",
//     "lastname": "Gragasin",
//     "mobile": "09123456789"
//     }
//   }
// }
