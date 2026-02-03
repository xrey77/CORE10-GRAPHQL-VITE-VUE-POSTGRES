    using core10_graphql_postgres.Entities;
    using core10_graphql_postgres.Helpers;
    using HotChocolate; 

    namespace core10_graphql_postgres.GraphQL.Mutations;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
        [UseMutationConvention]
        public async Task<User> Signup(SignupInput input, 
            [Service] GraphqlDbContext context)        
        {

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
                RolesId = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        return user;
        }
    }

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation Signup($input: SignupInput!) {
//   signup(input: $input) {
//       id
//       firstname
//       lastname
//       email
//       mobile
//       username      
//       isactivated
//       isblocked    
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
