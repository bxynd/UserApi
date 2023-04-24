using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApi.Entities;
using UserApi.Exceptions;
using UserApi.Services;

namespace UserApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateDto user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        return Ok(await _userService.CreateUser(user));
    }
    [Authorize(Roles = "Admin,Regular")]
    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update([FromRoute]Guid id,[FromBody]UpdateDto user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        try
        {
            return Ok(await _userService.Update(user, id));
        }
        catch(Exception e)
        {
            return Unauthorized("Unauthorized");
        }
    }
    [Authorize(Roles = "Admin,Regular")]
    [HttpPut("password/{id:Guid}")]
    public async Task<IActionResult> UpdatePassword([FromRoute]Guid id,[FromBody]string password)
    {
        if (password == "" && password.Length < 8 && Regex.IsMatch(password, "^[a-zA-Z0-9]*$"))
        {
            return BadRequest();
        }
        try
        {
            return Ok(await _userService.UpdatePassword(password, id));
        }
        catch (Exception e)
        {
            return Unauthorized("Unauthorized");
        }
    }
    [Authorize(Roles = "Admin,Regular")]
    [HttpPut("login/{id:Guid}")]
    public async Task<IActionResult> UpdateLogin([FromRoute]Guid id,[FromBody]string login)
    {
        if (login == "" && login.Length < 4 && Regex.IsMatch(login, "^[а-яА-ЯёЁ_a-zA-Z ]*"))
        {
            return BadRequest();
        }
        try
        {
            return Ok(await _userService.UpdateLogin(login, id));
        }
        catch (LoginOccupiedException e)
        {
            return BadRequest("This login is occupied");
        }
        catch (Exception e)
        {
            return Unauthorized("Unauthorized");
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllActive()
    {
        return Ok(await _userService.ReadAllActive());
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("{login:regex(^[[a-zA-Z0-9_]]*$)}")]
    public async Task<IActionResult> GetByLogin([FromRoute] string login)
    {
        return Ok(await _userService.ReadByLogin(login));
    }
    [Authorize]
    [HttpPost("by_login_password")] 
    public async Task<IActionResult> GetByLoginAndPassword([FromBody]string password)
    {
        try
        {
            return Ok(await _userService.ReadByLoginPassword(password));
        }
        catch (Exception e)
        {
            return NotFound("Not found");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("older/{months:int}")]
    public async Task<IActionResult> GetAllOlderThan(int months)
    {
        return Ok(await _userService.ReadAllOlderThan(months));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{login:regex(^[[a-zA-Z0-9_]]*$)}")]
    public async Task<IActionResult> Delete([FromRoute] string login)
    {
        return Ok(await _userService.DeleteByLogin(login));
    }
    [Authorize(Roles = "Admin")]
    [HttpPatch("restore/{id:Guid}")]
    public async Task<IActionResult> RestoreUser([FromRoute] Guid id)
    {
        return Ok(await _userService.RestoreUser(id));
    }
}