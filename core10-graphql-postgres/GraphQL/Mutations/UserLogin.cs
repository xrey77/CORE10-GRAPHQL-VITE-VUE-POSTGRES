using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Microsoft.EntityFrameworkCore; 
namespace core10_graphql_postgres.GraphQL.Mutations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

public record SigninInput(string Username, string Password);

public record SigninResponse(
    string Firstname, string Lastname, string Email, string Mobile,
    string Username, int Isactivated, int Isblocked, string Profilepic,
    string Qrcodeurl, string Rolename, string Token);

[ExtendObjectType("Mutation")]
public class UserLogin
{
    IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();


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

        var role = context.Roles.Where(r => r.Id == userDtls.Id).FirstOrDefault();
        userDtls.Rolename = role?.Name ?? "No Role Assigned";

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userDtls.Id.ToString()),
                new Claim(ClaimTypes.Name, userDtls.Username)
            }),
            Expires = DateTime.UtcNow.AddHours(1), // Set token expiration
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);


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
            userDtls.Rolename,
            tokenString
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
//       profilepic
//       rolename
//       token
//   }
// }

// GraphQL Variables
// {
//     "input": {
//       "username": "Rey",
//       "password": "rey"
//     }
// }  
