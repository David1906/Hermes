using Hermes.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace Hermes.Repositories;

public class HermesLocalContext : DbContext
{
    protected string ConnectionString { get; init; }

    public DbSet<Defect> Defects { get; set; }
    public DbSet<SfcResponse> SfcResponses { get; set; }
    public DbSet<Stop> Stops { get; set; }
    public DbSet<UnitUnderTest> UnitsUnderTest { get; set; }
    public DbSet<User> Users { get; set; }

    public HermesLocalContext()
    {
        var fullPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!,
            "dbSqlite.db");
        ConnectionString = $"Filename={fullPath}";
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
        modelBuilder.Entity<Defect>().ToTable("Defects");
        modelBuilder.Entity<SfcResponse>().ToTable("SfcResponses");
        modelBuilder.Entity<Stop>().ToTable("Stops");
        modelBuilder.Entity<UnitUnderTest>().ToTable("UnitsUnderTest");
        modelBuilder.Entity<User>().ToTable("Users");
    }
}