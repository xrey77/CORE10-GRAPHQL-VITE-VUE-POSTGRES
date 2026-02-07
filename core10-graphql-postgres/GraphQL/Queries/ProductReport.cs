using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate.Authorization;

namespace core10_graphql_postgres.GraphQL.Queries;

[ExtendObjectType("Query")]
public class ProductReport
{
    [AllowAnonymous]
    [UseProjection]    
    [UseFiltering]
    [UseSorting]    

    public IQueryable<Product> PdfReport(GraphqlDbContext context) {

        var products = context.Products;
        return products;
    }
}

// ======Nitro Request======
// query {
//   pdfReport {
//     id        
//     descriptions
//     qty
//     unit
//     costprice
//     sellprice
//     productpicture
//   }
// }

