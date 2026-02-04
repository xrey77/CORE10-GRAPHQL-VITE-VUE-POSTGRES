using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core10_graphql_postgres.Entities;

[Table("sales")]
public class Sale 
{
   [Key]
   [Column("id")]
   public int Id { get; set; }

   [Column(TypeName = "decimal(18,2)")] 
   public decimal Amount { get; set; }

   [Column("monthdate", TypeName = "date")]   
   public DateOnly Monthdate { get; set; }
}