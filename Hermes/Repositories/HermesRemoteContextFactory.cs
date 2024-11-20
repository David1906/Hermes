using Hermes.Models;
using Microsoft.EntityFrameworkCore.Design;

namespace Hermes.Repositories;

public class HermesRemoteContextFactory : IDesignTimeDbContextFactory<HermesRemoteContext>
{
    public HermesRemoteContext CreateDbContext(string[] args)
    {
        return new HermesRemoteContext(new Settings());
    }
}