using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate; 
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace core10_graphql_postgres.GraphQL.Mutations;

public record RegistrationResponse(User User, string Message);
public record SignupInput(
    string Firstname, 
    string Lastname, 
    string Email, 
    string Mobile, 
    string Username, 
    string Password
);

[ExtendObjectType("Mutation")]
public class UserRegistration
{
    IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
    [UseMutationConvention]
    public async Task<RegistrationResponse> Signup(SignupInput input, 
        [Service] GraphqlDbContext context)        
    {
        var keyString = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
        var key = Encoding.ASCII.GetBytes(keyString);
        var tokenHandler = new JwtSecurityTokenHandler();

        User userEmail = context.Users.Where(c => c.Email == input.Email).FirstOrDefault();
        if (userEmail is not null) {
            throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Email Address was already taken...")
                        .SetCode("DUPLICATE_EMAIL")
                        .Build());                
        }

        User userName = context.Users.Where(c => c.Username == input.Username).FirstOrDefault();
        if (userName is not null) {
            throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Username was already taken...")
                        .SetCode("DUPLICATE_EMAIL")
                        .Build());                                
        }



        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new(ClaimTypes.Name, input.Email)]),
            Expires = DateTime.UtcNow.AddHours(4),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var secretKey = tokenHandler.WriteToken(token);
        Console.WriteLine("SECRET........................." + secretKey);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);
        var user = new User {  
            Firstname = input.Firstname,
            Lastname = input.Lastname,
            Email = input.Email,
            Mobile = input.Mobile,
            Username = input.Username,
            Password = hashedPassword,
            Isactivated = 1,
            Isblocked = 0,
            Mailtoken = 0,
            Profilepic = "pix.png",
            RolesId = 1,
            Secretkey = secretKey.ToUpper()
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return new RegistrationResponse(user, "You have registered successfully, please login now.");
    }
}

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation Signup($input: SignupInput!) {
//   signup(input: $input) {
//     message
//   }
// }


// {
//   "input": {
//     "firstname": "Rey",
//     "lastname": "Gragasin",
//     "email": "rey@yahoo.com",
//     "mobile": "09123456789",
//     "username": "Rey",
//     "password": "rey"
//   }
// }
