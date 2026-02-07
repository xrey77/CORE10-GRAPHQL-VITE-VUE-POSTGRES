using System.Text;
using core10_graphql_postgres.GraphQL.Mutations;
using core10_graphql_postgres.GraphQL.Queries;
using core10_graphql_postgres.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();

// Register with Hot Chocolate
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)    
    .AddQueryType(d => d.Name("Query"))
    .AddTypeExtension<GetallUsers>()
    .AddTypeExtension<GetUser>()
    .AddTypeExtension<ProductsList>()
    .AddTypeExtension<ProductSearch>()
    .AddTypeExtension<ProductReport>()
    .AddTypeExtension<ChartData>()
    .AddMutationType(d => d.Name("Mutation"))
    .AddMutationConventions()
    .AddTypeExtension<UserRegistration>()
    .AddTypeExtension<UserLogin>()
    .AddTypeExtension<ChangeUserPassword>()
    .AddTypeExtension<ActivateMfa>()
    .AddTypeExtension<UpdateUserProfile>()
    .AddTypeExtension<AddProduct>()
    .AddType<DateType>()
    .AddTypeExtension<AddSales>()
    .AddTypeExtension<OtpVerification>()
    .AddType<UploadType>()    
    .AddTypeExtension<ChangeProfilePic>()
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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GraphqlDbContext>();
    // await db.Database.MigrateAsync(); 
    await db.Database.EnsureCreatedAsync();
}

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
app.MapFallbackToFile("index.html");

app.Run();
