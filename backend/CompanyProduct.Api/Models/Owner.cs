using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyProduct.Api.Models;

[Table("owners")]
public class Owner
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    public required string Username { get; set; }

    [Column("password_hash")]
    public required string PasswordHash { get; set; }
}