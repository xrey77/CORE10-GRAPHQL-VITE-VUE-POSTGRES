using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;

namespace core10_graphql_postgres.GraphQL.Queries;

[ExtendObjectType("Query")]
public class ChartData
{
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

