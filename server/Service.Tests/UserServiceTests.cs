using Microsoft.AspNetCore.Identity;

namespace Service.Tests;

public class UserServiceTests : TestData
{
    private const string Password = "CorrectPassword123!";
    private readonly FakePasswordHasher hasher = new();
    private UserService CreateService() => new(Db, hasher);

    [Fact]
    public void Registration_normalizes_username_and_stores_hash_from_hasher()
    {
        var response = CreateService().Register(new() { Username = "Alice", Password = Password });
        Assert.NotNull(response);
        var saved = Db.Users.Single();
        Assert.Equal(response.Id, saved.Id);
        Assert.Equal("Alice", saved.Username);
        Assert.Equal("ALICE", saved.NormalizedUsername);
        Assert.Equal(FakePasswordHasher.NewHash, saved.PasswordHash);
        Assert.Equal(1, hasher.HashCalls);
        Assert.Equal(Password, hasher.LastPassword);
    }

    [Theory]
    [InlineData("Alice")]
    [InlineData("alice")]
    [InlineData("ALICE")]
    public void Duplicate_username_is_rejected_without_hashing_or_inserting(string username)
    {
        AddUser("alice", "Alice");
        var insertCalls = Db.InsertCalls;
        Assert.Null(CreateService().Register(new() { Username = username, Password = Password }));
        Assert.Single(Db.Users.ToList());
        Assert.Equal(insertCalls, Db.InsertCalls);
        Assert.Equal(0, hasher.HashCalls);
    }

    [Theory]
    [InlineData("Alice")]
    [InlineData("alice")]
    public void Login_is_case_insensitive_and_delegates_password_verification(string username)
    {
        var user = AddUser("alice", "Alice");
        user.PasswordHash = "existing-hash";
        Db.Update(user);
        var updateCalls = Db.UpdateCalls;

        var response = CreateService().Login(new() { Username = username, Password = Password });

        Assert.NotNull(response);
        Assert.Equal(user.Id, response.Id);
        Assert.Equal(1, hasher.VerifyCalls);
        Assert.Equal("existing-hash", hasher.LastVerifiedHash);
        Assert.Equal(Password, hasher.LastPassword);
        Assert.Equal(0, hasher.HashCalls);
        Assert.Equal(updateCalls, Db.UpdateCalls);
    }

    [Fact]
    public void Failed_password_verification_rejects_login_without_updating_user()
    {
        AddUser("alice", "Alice");
        hasher.VerificationResult = PasswordVerificationResult.Failed;
        Assert.Null(CreateService().Login(new() { Username = "Alice", Password = "wrong-password" }));
        Assert.Equal(1, hasher.VerifyCalls);
        Assert.Equal(0, hasher.HashCalls);
        Assert.Equal(0, Db.UpdateCalls);
    }

    [Fact]
    public void Unknown_user_is_rejected_without_password_verification()
    {
        Assert.Null(CreateService().Login(new() { Username = "Unknown", Password = Password }));
        Assert.Equal(0, hasher.VerifyCalls);
        Assert.Equal(0, Db.UpdateCalls);
    }

    [Fact]
    public void Login_rehashes_and_updates_user_when_hasher_requests_upgrade()
    {
        AddUser("alice", "Alice");
        hasher.VerificationResult = PasswordVerificationResult.SuccessRehashNeeded;

        var response = CreateService().Login(new() { Username = "Alice", Password = Password });

        Assert.NotNull(response);
        Assert.Equal(1, hasher.HashCalls);
        Assert.Equal(1, Db.UpdateCalls);
        Assert.Equal(FakePasswordHasher.NewHash, Db.Users.Single().PasswordHash);
    }

    [Fact]
    public void User_lookup_returns_existing_user_and_null_for_unknown_id()
    {
        AddUser("alice", "Alice");
        var service = CreateService();
        Assert.Equal(new UserResponseDto("alice", "Alice"), service.GetById("alice"));
        Assert.Null(service.GetById("missing"));
    }
}
