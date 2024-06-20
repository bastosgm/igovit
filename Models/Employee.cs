using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace igovit.Models;
public class EmployeeDto
{
    public Guid? EmployeeId { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
}

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid EmployeeId { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public string? Name { get; set; }

    [ForeignKey("Unit")]
    public string? Unit { get; set; }
}

