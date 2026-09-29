using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;
using Microsoft.AspNetCore.Identity;

namespace Service;

public class UserService(MyDatabaseConnection db, IPasswordHasher<User> passwordHasher)
{
    public UserResponseDto? Register(RegisterRequestDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = dto.Username,
            NormalizedUsername = dto.Username.ToUpperInvariant()
        };

        if (db.Users.Any(existing => existing.NormalizedUsername == user.NormalizedUsername))
            return null;

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        db.Insert(user);

        return new UserResponseDto(user.Id, user.Username);
    }

    public UserResponseDto? Login(LoginRequestDto dto)
    {
        var normalized = dto.Username.ToUpperInvariant();
        var user = db.Users.FirstOrDefault(user => user.NormalizedUsername == normalized);
        if (user is null) return null;

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return null;

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
            db.Update(user);
        }

        return new UserResponseDto(user.Id, user.Username);
    }

}
