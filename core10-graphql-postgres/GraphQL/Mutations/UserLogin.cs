using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Microsoft.EntityFrameworkCore; 
namespace core10_graphql_postgres.GraphQL.Mutations;

public record SigninInput(string Username, string Password);

public record SigninResponse(
    string Firstname, string Lastname, string Email, string Mobile,
    string Username, int Isactivated, int Isblocked, string Profilepic,
    string Qrcodeurl, string Rolename);

[ExtendObjectType("Mutation")]
public class UserLogin
{
    [UseMutationConvention]
    public async Task<SigninResponse> Signin(
        SigninInput input, 
        [Service] GraphqlDbContext context)        
    {
        var userDtls = await context.Users
            .FirstOrDefaultAsync(c => c.Username == input.Username);

        if (userDtls == null) 
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("Username not found, please register first.")
                    .SetCode("USERNAME_NOT_FOUND")
                    .Build());
        }

        if (!BCrypt.Net.BCrypt.Verify(input.Password, userDtls.Password)) 
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("Invalid Password.")
                    .SetCode("INVALID_PASSWORD")
                    .Build());
        }

        if (userDtls.Isactivated == 0) 
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("Please activate your account via email.")
                    .SetCode("ACCOUNT_NOT_ACTIVATED")
                    .Build());
        }

        if (userDtls.Isblocked == 1) 
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("You Account has been blocked, please contact WEB MASTER.")
                    .SetCode("ACCOUNT_BLOCKED")
                    .Build());
        }

        Role role = context.Roles.Where(r => r.Id == userDtls.Id).FirstOrDefault();
        userDtls.Rolename = role?.Name ?? "No Role Assigned";

        return new SigninResponse(
            userDtls.Firstname,
            userDtls.Lastname,
            userDtls.Email,
            userDtls.Mobile,
            userDtls.Username,
            userDtls.Isactivated,
            userDtls.Isblocked,
            userDtls.Profilepic,
            userDtls.Qrcodeurl,
            userDtls.Rolename
            );
    }
}

// =======Nitro GraphQL Request===========
// mutation Sigin($input: SigninInput!) {
//   signin(input: $input) {
//       firstname
//       lastname
//       email
//       mobile
//       username      
//       isactivated
//       isblocked    
//   }
// }

// GraphQL Variables
// {
//     "input": {
//       "username": "Rey",
//       "password": "rey"
//     }
// }  
