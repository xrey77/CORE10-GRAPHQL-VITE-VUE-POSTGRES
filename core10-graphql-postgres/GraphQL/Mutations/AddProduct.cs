using core10_graphql_postgres.Entities;
using core10_graphql_postgres.Helpers;
using HotChocolate; 
using Microsoft.EntityFrameworkCore;

namespace core10_graphql_postgres.GraphQL.Mutations;

public record ProductResponse(Product Product, string Message);
public record ProductInput(
    string Category, 
    string Descriptions, 
    int Qty, 
    string Unit, 
    decimal Costprice, 
    decimal Sellprice,
    decimal Saleprice,
    string Productpicture,
    int Alertstocks,
    int Criticalstocks
);

[ExtendObjectType("Mutation")]
public class AddProduct
{
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]        
    [UseMutationConvention]
    public async Task<ProductResponse> CreateProduct(ProductInput input, 
        [Service] GraphqlDbContext context)        
    {

        var productDesc = context.Products.Where(c => c.Descriptions == input.Descriptions).FirstOrDefault();
        if (productDesc is not null) {
            throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Product Description was already taken...")
                        .SetCode("DUPLICATE_EMAIL")
                        .Build());                
        }

        var product = new Product
        {
            Category = input.Category,
            Descriptions = input.Descriptions,
            Qty = input.Qty,
            Unit = input.Unit,
            Costprice = input.Costprice,
            Sellprice = input.Sellprice,
            Saleprice = input.Saleprice,
            Productpicture = input.Productpicture,
            Alertstocks = input.Alertstocks,
            Criticalstocks = input.Criticalstocks
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();
        return new ProductResponse(product, "Product created successfully.");
    }
}

// ======Execute in Nitro, Request and GraphQL Variables=======
// mutation AddProduct($input: ProductInput!) {
//   createProduct(input: $input) {
//     message
//   }
// }


// {
//   "input": {
//     "category": "Luxury Cars",
//     "descriptions": "Sample Car 1",
//     "qty": 100,
//     "unit": "Units",
//     "costprice": 5000000,
//     "sellprice": 4500000,
//     "saleprice": 4900000,
//     "productpicture": "1.png",
//     "alertstocks": 40,
//     "criticalstocks": 10
//   }
// }
