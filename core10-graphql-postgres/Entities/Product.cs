using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core10_graphql_postgres.Entities;
    
[Table("products")]
public class Product {

        [Key]
        [Column("id")]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Descriptions { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public decimal Costprice { get; set; }
        public decimal Sellprice { get; set; }
        public string Productpicture { get; set; }
        public decimal Saleprice { get; set; }
        public int Alertstocks { get; set; }
        public int Criticalstocks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
}    
