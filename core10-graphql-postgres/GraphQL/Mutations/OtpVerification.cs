using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Google.Authenticator;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Mutations;

    public record TotpInput(int Id, string Otp);
    public record TotpMessage(User User, string Username, string Message);

    [ExtendObjectType("Mutation")]
    public class OtpVerification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
        [UseMutationConvention]
        public async Task<TotpMessage> VerifyTotp(
            TotpInput input, 
            [Service] GraphqlDbContext context)        
        {

            if (input.Otp is null)
            {
                    ErrorBuilder.New()
                        .SetMessage("Please enter OTP Code.")
                        .SetCode("EMPTY OTP")
                        .Build();
            }

            var user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == input.Id);

            if (user.Username is null) {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("User ID not found.")
                        .SetCode("USER ID_NOT_FOUND")
                        .Build());
            }

            TwoFactorAuthenticator twoFactor =  new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(user.Secretkey, input.Otp , false);
            if (isValid)
            {
                return new TotpMessage(user, user.Username, "OTP validation is successfull, please wait..");            
            } 
            else
            {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Invalid OTP code, please try again.")
                        .SetCode("INVALID OTP")
                        .Build());                        
            }
        }
    }

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation OtpVerification($input: TotpInput!) {
//   verifyTotp(input: $input) {
//     message
//   }
// }

// GraphQL Variables
// {
//   "input": {
//     "id": 1,
//     "otp": "123456"
//   }
// }
