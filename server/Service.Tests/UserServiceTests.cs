using Infra;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using LinqToDB;

namespace Service.Tests;

public class UserServiceTests : TestDatabase
{
    private const string Password = "CorrectPassword123!";
    private UserService CreateService() => new(Db, new PasswordHasher<User>());

    [Fact]
    public void Registration_stores_verifiable_hash_instead_of_plaintext()
    {
        var response = CreateService().Register(new() { Username = "Alice", Password = Password });
        Assert.NotNull(response);
        var saved = Db.Users.Single();
        Assert.Equal(response.Id, saved.Id);
        Assert.Equal("Alice", saved.Username);
        Assert.Equal("ALICE", saved.NormalizedUsername);
        Assert.NotEqual(Password, saved.PasswordHash);
        Assert.Equal(PasswordVerificationResult.Success,
            new PasswordHasher<User>().VerifyHashedPassword(saved, saved.PasswordHash, Password));
    }

    [Theory]
    [InlineData("Alice")]
    [InlineData("alice")]
    [InlineData("ALICE")]
    public void Duplicate_username_is_rejected_regardless_of_case(string username)
    {
        var service = CreateService();
        service.Register(new() { Username = "Alice", Password = Password });
        Assert.Null(service.Register(new() { Username = username, Password = "AnotherPassword123!" }));
        Assert.Single(Db.Users.ToList());
        Assert.NotNull(service.Login(new() { Username = "Alice", Password = Password }));
    }

    [Theory]
    [InlineData("Alice", Password, true)]
    [InlineData("alice", Password, true)]
    [InlineData("Alice", "WrongPassword123!", false)]
    [InlineData("Unknown", Password, false)]
    public void Login_requires_matching_username_and_password(string username, string password, bool succeeds)
    {
        var service = CreateService();
        var registered = service.Register(new() { Username = "Alice", Password = Password });
        var response = service.Login(new() { Username = username, Password = password });
        if (succeeds)
        {
            Assert.NotNull(response);
            Assert.Equal(registered!.Id, response.Id);
        }
        else Assert.Null(response);
    }

    [Fact]
    public void Login_upgrades_older_password_hash_and_keeps_password_valid()
    {
        var user = AddUser("alice", "Alice");
        var oldHasher = new PasswordHasher<User>(Options.Create(new PasswordHasherOptions { IterationCount = 1000 }));
        user.PasswordHash = oldHasher.HashPassword(user, Password);
        Db.Update(user);
        var oldHash = user.PasswordHash;

        var service = CreateService();
        Assert.NotNull(service.Login(new() { Username = "Alice", Password = Password }));
        var saved = Db.Users.Single();
        Assert.NotEqual(oldHash, saved.PasswordHash);
        Assert.Equal(PasswordVerificationResult.Success,
            new PasswordHasher<User>().VerifyHashedPassword(saved, saved.PasswordHash, Password));
        Assert.NotNull(service.Login(new() { Username = "Alice", Password = Password }));
    }

    [Fact]
    public void User_lookup_returns_existing_user_and_null_for_unknown_id()
    {
        var service = CreateService();
        var registered = service.Register(new() { Username = "Alice", Password = Password });
        Assert.Equal(registered, service.GetById(registered!.Id));
        Assert.Null(service.GetById("missing"));
    }
}
