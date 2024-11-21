using Hermes.Cipher.Types;
using Hermes.Models;
using Hermes.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Hermes.Repositories;

public class UserLocalRepository(IDbContextFactory<HermesLocalContext> context) : IReadOnlyUserRepository
{
    public async Task<IEnumerable<User>> FindAll(DepartmentType department, UserLevel sessionUserLevel)
    {
        await using var ctx = await context.CreateDbContextAsync();
        return await FindAllUsersQuery(ctx, department, sessionUserLevel).ToListAsync();
    }

    public async Task<User> FindById(string searchEmployeeId)
    {
        await using var ctx = await context.CreateDbContextAsync();
        return await ctx.Users.FirstOrDefaultAsync(x => x.EmployeeId == searchEmployeeId) ?? User.Null;
    }

    public async Task<IEnumerable<User>> Find(string searchEmployeeId, DepartmentType department,
        UserLevel sessionUserLevel)
    {
        await using var ctx = await context.CreateDbContextAsync();
        return await FindAllUsersQuery(ctx, department, sessionUserLevel)
            .Where(x => x.EmployeeId.Contains(searchEmployeeId))
            .ToListAsync();
    }

    private IQueryable<User> FindAllUsersQuery(HermesLocalContext ctx, DepartmentType department,
        UserLevel sessionUserLevel)
    {
        return ctx.Users
            .Where(x => department == DepartmentType.Admin ||
                        (x.Department == department && x.Level < sessionUserLevel));
    }

    public async Task<User> FindUser(string userName, string password)
    {
        await using var ctx = await context.CreateDbContextAsync();
        var user = await ctx.Users
            .Where(x => x.EmployeeId == userName && x.Password == password)
            .FirstOrDefaultAsync();
        if (user == null) return User.Null;
        user.Permissions = await ctx.FeaturePermissions
            .Where(x => x.Department <= user.Department && x.Level <= user.Level).ToListAsync();
        return user;
    }

    public async Task<bool> IsOnline()
    {
        try
        {
            var ctx = await context.CreateDbContextAsync();
            return await ctx.Database.CanConnectAsync();
        }
        catch (Exception)
        {
            return false;
        }
    }
}