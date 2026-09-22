using CompanyProduct.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyProduct.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Owner> Owners => Set<Owner>();
}