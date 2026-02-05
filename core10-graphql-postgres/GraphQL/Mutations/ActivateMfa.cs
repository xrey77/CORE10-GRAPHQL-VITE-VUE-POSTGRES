using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Google.Authenticator;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Mutations;

    public record ActivationInput(
        [property: ID] int Id,
        Boolean TwoFactorEnabled);
    public record ActivationResponse(User User, string Qrcode, string Message);

    [ExtendObjectType("Mutation")]
    public class ActivateMfa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
        [UseMutationConvention]
        public async Task<ActivationResponse> MfaActivationAsync(
            ActivationInput input, 
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

            if (input.TwoFactorEnabled == true)
            {
                var companyName = "Apple Inc.";
                TwoFactorAuthenticator twoFactor = new();
                var setupInfo = twoFactor.GenerateSetupCode(companyName, user.Email, user.Secretkey, false, 3);
                var imageUrl = setupInfo.QrCodeSetupImageUrl;
                user.Qrcodeurl = imageUrl;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return new ActivationResponse(user, imageUrl, "Multi-Factor Authenticator has been enabled.");
            } 
            else
            {
                user.Qrcodeurl = null;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return new ActivationResponse(user, "", "Multi-Factor Authenticator has been disabled.");                
            }
        }
    }

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation ActivateMfa($input: MfaActivationInput!) {
//   mfaActivation(input: $input) {
//   }
// }    // activationResponse {
    //   user {
    //     id        
    //   }
    //   message
    // }



// GraphQL Variables
// {
//   "input": {    
//     "input": {
//     "id": 1,
//     "twoFactorEnabled": false
//     }
//   }
// }
