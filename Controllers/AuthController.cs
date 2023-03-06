using System.Security.Cryptography;
using System.Text;
using Chat.Auth.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    public static User user = new User();
    // public AuthController(){

    // }

    [HttpPost("register")]
    public ActionResult<UserData> Register(UserData userInfo)
    {
        CreatePasswordHash(userInfo.Password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        return Ok(user);
    }
    [HttpPost("login")]
    public ActionResult<UserData> Login(UserData request)
    {
        if (user is null)
        {
            return BadRequest("user not found");
        }
        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return BadRequest("wrong password");
        }
        return Ok(user);

    }
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);

        }

    }
}