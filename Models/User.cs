using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace igovit.Models;

public class UserDto
{
    public string? Login { get; set; }
    public string? Status { get; set; }
}
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Status { get; set; }
}