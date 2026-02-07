using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Mutations;

public record SaleResponse(Sale Sale, string Message);
public record SaleInput(
    decimal Amount,
    DateOnly Monthdate
);

[ExtendObjectType("Mutation")]
public class AddSales
{
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
    [Authorize]
    [UseMutationConvention]
    public async Task<SaleResponse> CreateSale(SaleInput input, 
        [Service] GraphqlDbContext context)        
    {

        var exists = await context.Sales.AnyAsync(c => c.Monthdate == input.Monthdate);
        
        if (exists) {
            throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Date already exists...")
                        .SetCode("DUPLICATE_DATE")
                        .Build());                      
        }

        var sale = new Sale
        {
            Amount = input.Amount,
            Monthdate = input.Monthdate
        };

        context.Sales.Add(sale);
        await context.SaveChangesAsync();
        return new SaleResponse(sale, "Sale created successfully.");
    }
} 

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation AddSales($input: SaleInput!) {
//   createSale(input: $input) {
//     message
//   }
// }

//======GraphQL Variables======================================
//  {
//   "input": {
//     "amount": 50000,
//     "monthdate": "2025-01-30"
//   }
// }
