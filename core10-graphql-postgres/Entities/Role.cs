using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core10_graphql_postgres.Entities;

[Table("roles")]
public class Role {

    [Key]
    [Column("id")]
    public int Id {get; set;}

    public string Name {get; set;}

    public ICollection<User> Users { get; set; } = new List<User>();
}
