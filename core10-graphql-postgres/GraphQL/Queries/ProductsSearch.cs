using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Queries;

    public record KeyInput(int Page, string Keyword);
    public record SearchResponse(IEnumerable<Product> Products, int Page, int Totalpage, int Totalrecords);

[ExtendObjectType("Query")]
public class ProductSearch
{
    [AllowAnonymous]
    public async Task<SearchResponse> SearchProducts(
        KeyInput input, 
        [Service] GraphqlDbContext context)        
    {
        var perPage = 5;
        
        // Base query for reuse
        var query = context.Products
            .Where(m => EF.Functions.ILike(m.Descriptions, $"%{input.Keyword}%"));

        var totalRecords = await query.CountAsync();                
        if (totalRecords == 0) {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("No record(s) found.")
                    .SetCode("NO_RECORDS_FOUND")
                    .Build());
        }

        int totpage = (int)Math.Ceiling((float)totalRecords / perPage);
        var offset = (input.Page - 1) * perPage;
        
        var products = await query
            .OrderBy(b => b.Id)
            .Skip(offset)
            .Take(perPage)
            .ToListAsync();

        return new SearchResponse(products, input.Page, totpage, totalRecords);
    }
}


// ======Execute in Nitro, Request and GraphQL Variables=======
// query ProductSearch($input: KeyInput!) {
//   searchProducts(input: $input) {
//    products {
//     id
//     descriptions
//     qty
//     unit
//     costprice
//     sellprice
//     productpicture
//    }
//    totalpage
//    page
//    totalrecords
//   }
// }

// ========= GraphQL Variables================================
// {
//   "input": {
//     "page": 1,
//     "keyword": "sample car 35"    
//   }
// }
