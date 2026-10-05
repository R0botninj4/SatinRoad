using Infra;
using Microsoft.AspNetCore.Identity;

namespace Service.Tests;

public class FakePasswordHasher : IPasswordHasher<User>
{
    public const string NewHash = "hash-returned-by-test-double";
    public PasswordVerificationResult VerificationResult { get; set; } = PasswordVerificationResult.Success;
    public int HashCalls { get; private set; }
    public int VerifyCalls { get; private set; }
    public string? LastPassword { get; private set; }
    public string? LastVerifiedHash { get; private set; }

    public string HashPassword(User user, string password)
    {
        HashCalls++;
        LastPassword = password;
        return NewHash;
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        VerifyCalls++;
        LastVerifiedHash = hashedPassword;
        LastPassword = providedPassword;
        return VerificationResult;
    }
}
