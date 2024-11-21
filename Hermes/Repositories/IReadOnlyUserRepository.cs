using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hermes.Cipher.Types;
using Hermes.Models;
using Hermes.Types;

namespace Hermes.Repositories;

public interface IReadOnlyUserRepository
{
    Task<IEnumerable<User>> FindAll(DepartmentType department, UserLevel sessionUserLevel);
    Task<User> FindById(string searchEmployeeId);

    Task<IEnumerable<User>> Find(string searchEmployeeId, DepartmentType department,
        UserLevel sessionUserLevel);

    Task<User> FindUser(string userName, string password);

    Task<bool> IsOnline();
}