using Hermes.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hermes.Repositories;

public class HermesRemoteContext : DbContext
{
    private const string DatabaseName = "hermes";
    private const string User = "hermes";
    private const string Password = "AmazingPassword";
    private string _server = "localhost";

    public string ConnectionString => $"Server={_server};Database={DatabaseName};user={User};password={Password}";
    public DbSet<User> Users { get; set; }
    public DbSet<FeaturePermission> FeaturePermissions { get; set; }

    public HermesRemoteContext(Settings settings)
    {
#if !DEBUG
        _server = settings.DatabaseServer;
#endif
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
            connectionString: ConnectionString,
            serverVersion: ServerVersion.AutoDetect(ConnectionString));
        base.OnConfiguring(optionsBuilder);
    }

    public bool Migrate()
    {
        try
        {
            if (Database.GetPendingMigrations().Any())
            {
                Database.Migrate();
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<FeaturePermission>().ToTable("feature_permissions");
    }
}