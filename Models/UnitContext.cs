using Microsoft.EntityFrameworkCore;

namespace igovit.Models;

public class UnitContext : DbContext
{
    public UnitContext(DbContextOptions<UnitContext> options)
        : base(options)
    {
    }

    public DbSet<Unit> Units { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
}