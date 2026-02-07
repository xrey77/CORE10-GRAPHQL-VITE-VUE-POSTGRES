using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate.Authorization;

namespace core10_graphql_postgres.GraphQL.Queries;

[ExtendObjectType("Query")]
public class ChartData
{
    [AllowAnonymous]
    [UseProjection]
    [UseFiltering]
    [UseSorting]    

    public IQueryable<Sale> Salesdata(GraphqlDbContext context) {
        var sales = context.Sales;
        return sales;        
    }
}

// ======Nitro Request======
// query {
//   salesdata {
//     amount
//     monthdate
//   }
// }

