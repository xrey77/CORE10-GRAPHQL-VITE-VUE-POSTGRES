using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using Path = System.IO.Path;

namespace core10_graphql_postgres.GraphQL.Mutations;

public class UploadResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
}


[ExtendObjectType("Mutation")]
public class ChangeProfilePic
{
    
    public async Task<UploadResponse> ProfilePicUploadAsync(
        int id,
        IFile profilepic,
        [Service] GraphqlDbContext dbContext, 
        IWebHostEnvironment env)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user == null)
        {
            throw new GraphQLException($"User with ID {id} not found");
        }

        var extension = Path.GetExtension(profilepic.Name);
        var newFile = $"00{id}{extension}";
        var uploadPath = Path.Combine(env.WebRootPath, "users", newFile);

        await using Stream stream = profilepic.OpenReadStream();
        using (var fileStream = new FileStream(uploadPath, FileMode.Create))
        {
            await stream.CopyToAsync(fileStream);
        }

        user.Profilepic = newFile;
        await dbContext.SaveChangesAsync();

        return new UploadResponse
        {
            Id = id,
            Message = "You have changed your profile picture successfully."
        };
    }
}


// =======Nitro GraphQL Request===========
// mutation ChangeProfilePic($input: ProfilePicUploadInput!) {
//     profilePicUpload(input: $input) {
//         uploadResponse {
//             id
//             message
//         }
//     }
// }


// =======GraphQL Variables======
// {
//     input: {
//         id: 1,
//         profilepic: null
//     }
// }