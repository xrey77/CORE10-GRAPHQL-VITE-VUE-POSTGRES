using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Queries;

    public record PageInput(int Page);
    public record PageResponse(IEnumerable<Product> Product, int Page, int Totalpage, int Totalrecords );

    [ExtendObjectType("Query")]
    public class ProductsList
    {
        [AllowAnonymous]
        public async Task<PageResponse> ListProducts(
            PageInput input, 
            [Service] GraphqlDbContext context)        
        {

            var perPage = 5;
            var totrecords = await context.Products.CountAsync();
            int totalpage = (int)Math.Ceiling((float)(totrecords) / perPage);

            var offset = (input.Page -1) * perPage;

            if (totrecords == 0) {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("No record(s) found.")
                        .SetCode("NO RECORDS_FOUND")
                        .Build());
            }

            var products = await context.Products
                .OrderBy(b => b.Id)
                .Skip(offset)
                .Take(perPage)
                .ToListAsync();


            return new PageResponse(products, input.Page, totalpage, totrecords);
        }
    }

// ======Execute in Nitro, Request and GraphQL Variables=======
// query ProductsList($input: PageInput!) {
//   listProducts(input: $input) {
//     product {
//       id
//       descriptions
//       qty
//       unit
//       costprice
//       sellprice
//       productpicture
//     }
//     page
//     totalpage
//     totalrecords
//   }
// }


// ========= GraphQL Variables================================
// {
//   "input": {
//     "page": 1
//   }
// }
