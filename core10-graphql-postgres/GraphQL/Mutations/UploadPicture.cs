using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using Path = System.IO.Path;

namespace core10_graphql_postgres.GraphQL.Mutations;

public record UploadMessage(User User, string Message);

[ExtendObjectType("Mutation")]
public class UploadPicture
{
    public async Task<UploadMessage> UpdateProfilepic(
        int id,
        IFile  profilepic,
        [Service] GraphqlDbContext context)        
    {
        var user = await context.Users.FindAsync(id) ?? throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("User ID not found.")
                .SetCode("USER_ID_NOT_FOUND")
                .Build());

        // Determine path
        string ext = Path.GetExtension(profilepic.Name);
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users");
        // Directory.CreateDirectory(folderPath);

        var fileName = $"00{id}{ext}";
        var fullPath = Path.Combine(folderPath, fileName);

        // Process image with ImageSharp
        using (var stream = profilepic.OpenReadStream())
        using (var image = await SixLabors.ImageSharp.Image.LoadAsync(stream))
        {
            image.Mutate(x => x.Resize(100, 100));
            await image.SaveAsync(fullPath);
        }

        user.Profilepic = fileName;
        await context.SaveChangesAsync();

        return new UploadMessage(user, "Profile picture updated successfully.");
    }
}

// =======Nitro GraphQL Request===========
// mutation UploadPicture($input: ImageInput!) {
//   updateProfilepic(input: $input) {
//      id
//      profilepic
//   }
// }

// mutation UpdateUserPic($id: Int!, $file: Upload!) {
//   updateProfilepic(input: { id: $id, profilepic: $file }) {
//     message
//     user {
//       id
//       profilepic
//     }
//   }
// }


// GraphQL Variables
// {
//     "input": {
//       "id": "Rey",
//       "profilepic": ""
//     }
// }  
