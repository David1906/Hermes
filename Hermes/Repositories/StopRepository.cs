using System;
using System.Collections.Generic;
using System.Linq;
using Hermes.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Hermes.Types;

namespace Hermes.Repositories;

public class StopRepository(IDbContextFactory<HermesLocalContext> context)
    : BaseRepository<Stop, HermesLocalContext>(context)
{
    private readonly IDbContextFactory<HermesLocalContext> _context = context;

    public async Task RestoreAsync(Stop stop, List<User> users)
    {
        await using var ctx = await _context.CreateDbContextAsync();
        var dbStop = await ctx.Stops.Where(x => x.Id == stop.Id).FirstOrDefaultAsync();
        if (dbStop == null) return;
        var dbUsers = await ctx.Users.Where(x => users.Contains(x)).ToListAsync();
        dbStop.IsRestored = true;
        dbStop.ClosedAt = DateTime.Now;
        dbStop.Users = dbUsers;
        await ctx.SaveChangesAsync();
    }

    public async Task<List<UnitUnderTest>> GetLast(
        string? serialNumberFilter,
        string? detailsFilter,
        StopType? stopType,
        TimeSpan? lastTimeSpan,
        int limit = 500)
    {
        var ctx = await _context.CreateDbContextAsync();
        var query = ctx.UnitsUnderTest
            .Include(x => x.Stop)
            .Include(x => x.Stop!.Users)
            .Where(x => x.Stop != null)
            .OrderByDescending(x => x.Id)
            .Take(limit)
            .AsQueryable();

        if (stopType != null)
        {
            query = query.Where(x => x.Stop!.Type == stopType);
        }

        if (lastTimeSpan != null)
        {
            var minDatetime = DateTime.Now - lastTimeSpan.Value;
            query = query.Where(x => x.CreatedAt >= minDatetime);
        }

        if (!string.IsNullOrWhiteSpace(serialNumberFilter))
        {
            query = query.Where(x => EF.Functions.Like(x.SerialNumber, $"%{serialNumberFilter}%"));
        }

        if (!string.IsNullOrWhiteSpace(detailsFilter))
        {
            query = query.Where(x => EF.Functions.Like(x.Stop!.Details, $"%{detailsFilter}%"));
        }

        return await query.ToListAsync();
    }
}