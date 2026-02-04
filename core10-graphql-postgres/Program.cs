using core10_graphql_postgres.GraphQL.Mutations;
using core10_graphql_postgres.GraphQL.Queries;
using core10_graphql_postgres.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPooledDbContextFactory<GraphqlDbContext>(options =>
{
    options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => 
            {
                npgsqlOptions.EnableRetryOnFailure(); 
            })
           .UseSnakeCaseNamingConvention();
});

// Register with Hot Chocolate
builder.Services
    .AddGraphQLServer()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)    
    .AddQueryType(d => d.Name("Query"))
    .AddTypeExtension<GetallUsers>()
    .AddTypeExtension<GetUser>()
    .AddTypeExtension<ProductsList>()
    .AddTypeExtension<ProductSearch>()
    .AddMutationType(d => d.Name("Mutation"))
    .AddMutationConventions()
    .AddTypeExtension<UserRegistration>()
    .AddTypeExtension<UserLogin>()
    .AddTypeExtension<ChangeUserPassword>()
    .AddTypeExtension<ActivateMfa>()
    .AddTypeExtension<UpdateProfile>()
    .AddTypeExtension<AddProduct>()
    .AddType<DateType>()
    .AddTypeExtension<AddSales>()
    .AddTypeExtension<OtpVerification>()
    .AddTypeExtension<UploadPicture>()
    .AddType<UploadType>()    
    .AddErrorFilter<GraphQLErrorFilter>()
    .RegisterDbContextFactory<GraphqlDbContext>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .WithMethods("GET", "POST","DELETE","PATCH","POST","OPTIONS")
              .WithExposedHeaders("Content-Disposition")
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});


var app = builder.Build();

// ===== AUTO CREATE TABLES ===============================
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<GraphqlDbContext>();
//     // await db.Database.MigrateAsync(); 
//     await db.Database.EnsureCreatedAsync();
// }

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseCors( options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapControllers();
app.MapGraphQL();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
