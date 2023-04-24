using System.Security.Claims;
using UserApi.Entities;
using UserApi.Exceptions;
using UserApi.Repositories;

namespace UserApi.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public UserService(IHttpContextAccessor httpContextAccessor,UserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<Guid?> CreateUser(CreateDto userDto)
    {
        var creatorLogin = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        
        var temp = await _userRepository.FindByLogin(userDto.Login);
        if (temp != null)
        {
            return null;
        }
        
        var user = new User
        {
            Login = userDto.Login,
            Password = userDto.Password,
            Name = userDto.Name,
            Gender = userDto.Gender,
            Birthday = userDto.Birthday,
            Admin = userDto.Admin,
            CreatedOn = DateTime.Now,
            CreatedBy = creatorLogin
        };
        return await _userRepository.Create(user);
    }

    public async Task<Guid?> Update(UpdateDto userDto,Guid id)
    {
        var editorLogin = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        var role = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        if (role == "Admin")
        {
            return await _userRepository.Update(userDto,id,editorLogin);
        }
        
        id = await _userRepository.FindByLoginReturnId(editorLogin);
        return await _userRepository.Update(userDto, id, editorLogin);
    }
    public async Task<Guid> UpdateLogin(string login,Guid id)
    {
        var isUnique = await _userRepository.FindByLogin(login) == null;
        if (!isUnique)
        {
            throw new LoginOccupiedException();
        }
        var editorLogin = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        var role = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        if (role == "Admin")
        {
            return await _userRepository.UpdateLogin(login,id,editorLogin);
        }
        
        id = await _userRepository.FindByLoginReturnId(editorLogin);
        return await _userRepository.UpdateLogin(login,id,editorLogin);

    }
    public async Task<Guid> UpdatePassword(string password,Guid id)
    {
        var editorLogin = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        var role = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        if (role == "Admin")
        {
            return await _userRepository.UpdatePassword(password, id, editorLogin);
        }
        id = await _userRepository.FindByLoginReturnId(editorLogin);
        return await _userRepository.UpdatePassword(password, id, editorLogin);
    }

    public async Task<List<User>> ReadAllActive()
    {
        return await _userRepository.ReadAllActive();
    }

    public async Task<User?> ReadByLoginPassword(string password)
    {
        var login = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

        var user = await _userRepository.FindByLoginPassword(login, password);
        if (user != null && user.RevokedOn == null)
        {
            return user;
        }
        throw new Exception();
    }
    public async Task<User?> ReadByLogin(string login)
    {
        return await _userRepository.FindByLogin(login);
    }
    public async Task<List<User>> ReadAllOlderThan(int months)
    {
        return await _userRepository.ReadAllOlderThan(months);
    }

    public async Task<Guid> DeleteByLogin(string login)
    {
        var editorLogin = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

        return await _userRepository.SoftDelete(editorLogin,login);
    }
    public async Task<Guid> RestoreUser(Guid id)
    {
        var editorLogin = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

        return await _userRepository.UpdateStatus(id,editorLogin);
    }
}