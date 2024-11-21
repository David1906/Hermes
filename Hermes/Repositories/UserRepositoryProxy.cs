using Hermes.Cipher.Types;
using Hermes.Language;
using Hermes.Models;
using Hermes.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Hermes.Repositories;

public class UserRepositoryProxy
{
    private IReadOnlyUserRepository _readOnlyUserRepository;
    private readonly Session _session;
    private readonly UserLocalRepository _userLocalRepository;
    private readonly UserRemoteRepository _userRemoteRepository;

    public UserRepositoryProxy(
        Session session,
        UserRemoteRepository userRemoteRepository,
        UserLocalRepository userLocalRepository)
    {
        this._session = session;
        this._userRemoteRepository = userRemoteRepository;
        this._userLocalRepository = userLocalRepository;
        this._readOnlyUserRepository = userRemoteRepository;
    }

    public async Task<IEnumerable<User>> FindAll(DepartmentType department, UserLevel sessionUserLevel)
    {
        await this.SelectRepository();
        return await _readOnlyUserRepository.FindAll(department, sessionUserLevel);
    }

    public async Task<IEnumerable<User>> FindById(string searchEmployeeId, DepartmentType department,
        UserLevel sessionUserLevel)
    {
        await this.SelectRepository();
        return await _readOnlyUserRepository.Find(searchEmployeeId, department, sessionUserLevel);
    }

    public async Task<User> FindUser(string userName, string password)
    {
        await this.SelectRepository();
        return await _readOnlyUserRepository.FindUser(userName, password);
    }

    private async Task SelectRepository()
    {
        this._session.IsDatabaseOnline.Value = await _userRemoteRepository.IsOnline();
        if (this._session.IsDatabaseOnline.Value)
        {
            this._readOnlyUserRepository = _userRemoteRepository;
        }
        else
        {
            this._readOnlyUserRepository = _userLocalRepository;
        }
    }

    public async Task<int> UpdateUser(User user)
    {
        return await _userRemoteRepository.UpdateUser(user);
    }

    public async Task Add(User user)
    {
        var userExists = await _userRemoteRepository.FindById(user.EmployeeId);
        if (!userExists.IsNull)
        {
            throw new Exception(Resources.msg_user_already_exists);
        }

        await _userRemoteRepository.AddAndSaveAsync(user);
    }

    public void Delete(User user)
    {
        _userRemoteRepository.Delete(user);
    }
}