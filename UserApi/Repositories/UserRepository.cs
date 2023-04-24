using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UserApi.Context;
using UserApi.Entities;

namespace UserApi.Repositories;

public class UserRepository
{
    private readonly DataContext _dataContext;

    public UserRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public async Task<Guid> Create(User user)
    {
        var res = await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        return res.Entity.Guid;
    }

    public async Task<List<User>> ReadAllActive()
    {
        return await _dataContext.Users.Where(x=>x.RevokedOn == null).AsNoTracking().OrderBy(x=>x.CreatedOn).ToListAsync();
    }
    public async Task<User?> FindByLoginPassword(string login, string password)
    {
        return await _dataContext.Users.Where(x=>x.Login == login && x.Password == password).FirstOrDefaultAsync();
    }
    public async Task<User?> FindByLogin(string login)
    {
        return await _dataContext.Users.Where(x=>x.Login == login).Select(a => new User
        {
            Name = a.Name,
            Gender = a.Gender,
            Birthday = a.Birthday,
            RevokedOn = a.RevokedOn
        }).FirstOrDefaultAsync();
    }
    public async Task<Guid> FindByLoginReturnId(string login)
    {
        return ((await _dataContext.Users.Where(u => u.Login == login).Select(a => new User
        {
            Guid = a.Guid
        }).FirstOrDefaultAsync())!).Guid;
    }  
    public async Task<List<User>> ReadAllOlderThan(int months)
    {
        return await _dataContext.Users.Where(x => (DateTime.Now - x.CreatedOn).Days >= months*30).AsNoTracking().ToListAsync();
    }
    public async Task<Guid> Update(UpdateDto userDto,Guid id,string editorLogin)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Guid == id);
        user.Name = userDto.Name;
        user.Gender = userDto.Gender;
        user.Birthday = userDto.Birthday;
        user.ModifiedBy = editorLogin;
        user.ModifiedDate = DateTime.Now;
        await _dataContext.SaveChangesAsync();
        return id;
    }
    public async Task<Guid> UpdatePassword(string password,Guid id,string editorLogin)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Guid == id);
        user.Password = password;
        user.ModifiedBy = editorLogin;
        user.ModifiedDate = DateTime.Now;
        await _dataContext.SaveChangesAsync();
        return id;
    }
    public async Task<Guid> UpdateLogin(string login,Guid id,string editorLogin)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Guid == id);
        user.Login = login;
        user.ModifiedBy = editorLogin;
        user.ModifiedDate = DateTime.Now;
        await _dataContext.SaveChangesAsync();
        return id;
    }

    public async Task<Guid> SoftDelete(string revokedBy,string login)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        user.RevokedBy = revokedBy;
        user.RevokedOn = DateTime.Now;
        await _dataContext.SaveChangesAsync();
        return user.Guid; 
    }
    public async Task<Guid> UpdateStatus(Guid id,string editorLogin)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Guid == id);
        user.ModifiedBy = editorLogin;
        user.ModifiedDate = DateTime.Now;
        user.RevokedBy = null;
        user.RevokedOn = null;
        await _dataContext.SaveChangesAsync();
        return id;
    }
}