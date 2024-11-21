using Hermes.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace Hermes.Repositories;

public class HermesLocalContext : DbContext
{
    public string FilePath { get; init; }
    public string ConnectionString { get; init; }
    public DbSet<Defect> Defects { get; set; }
    public DbSet<SfcResponse> SfcResponses { get; set; }
    public DbSet<Stop> Stops { get; set; }
    public DbSet<UnitUnderTest> UnitsUnderTest { get; set; }
    public DbSet<FeaturePermission> FeaturePermissions { get; set; }
    public DbSet<User> Users { get; set; }

    public HermesLocalContext()
    {
        FilePath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!,
            "dbSqlite.db");
        ConnectionString = $"Filename={FilePath}";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(connectionString: ConnectionString);
        base.OnConfiguring(optionsBuilder);
    }

    public void Migrate()
    {
#if DEBUG
        //Database.EnsureDeleted();
#endif
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Defect>().ToTable("defects");
        modelBuilder.Entity<SfcResponse>().ToTable("sfc_responses");
        modelBuilder.Entity<Stop>().ToTable("stops");
        modelBuilder.Entity<UnitUnderTest>().ToTable("units_under_test");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<FeaturePermission>().ToTable("feature_permissions");
        modelBuilder.Entity<User>()
            .HasMany(left => left.Stops)
            .WithMany(right => right.Users)
            .UsingEntity(join => join.ToTable("user_stop"));
    }
}