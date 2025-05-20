using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.Models;

namespace WindPowerSystemV5.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext() : base()
    {
    }

    public ApplicationDbContext(DbContextOptions options)
     : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // add the EntityTypeConfiguration classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public DbSet<City> Cities => Set<City>();

    public DbSet<Country> Countries => Set<Country>();

    public DbSet<TurbineType> TurbineTypes => Set<TurbineType>();

    public DbSet<Turbine> Turbines => Set<Turbine>();
}
