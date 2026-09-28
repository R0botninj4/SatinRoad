using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

namespace Service;

public class UserService(MyDatabaseConnection db, IPasswordHasher<User> passwordHasher)
{
    private static readonly User DummyUser = new();
    private static readonly string DummyHash = new PasswordHasher<User>()
        .HashPassword(DummyUser, Guid.NewGuid().ToString());

    public UserResponseDto? Register(RegisterRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = dto.Username,
            NormalizedUsername = dto.Username.ToUpperInvariant()
        };

        if (db.Users.Any(existing => existing.NormalizedUsername == user.NormalizedUsername))
            return null;

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        try
        {
            db.Insert(user);
        }
        catch (SqliteException exception) when (exception.SqliteExtendedErrorCode == 2067)
        {
            // The database also protects against concurrent registrations of the same name.
            return null;
        }

        return new UserResponseDto(user.Id, user.Username);
    }

    public UserResponseDto? Login(LoginRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        var normalized = dto.Username.ToUpperInvariant();
        var user = db.Users.FirstOrDefault(user => user.NormalizedUsername == normalized);
        if (user is null)
        {
            // Do password verification work for unknown names as well.
            passwordHasher.VerifyHashedPassword(DummyUser, DummyHash, dto.Password);
            return null;
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return null;

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
            db.Update(user);
        }

        return new UserResponseDto(user.Id, user.Username);
    }

    public UserResponseDto? GetById(string id)
    {
        return db.Users.Where(user => user.Id == id)
            .Select(user => new UserResponseDto(user.Id, user.Username)).FirstOrDefault();
    }
}
