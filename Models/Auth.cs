
namespace Chat.Auth.Models;

public class UserData
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class User
{
    public int Id { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}