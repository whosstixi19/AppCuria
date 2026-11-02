using AplicacionCuria.Security;
using Xunit;

namespace AplicacionCuria.Tests;

public class PasswordHelperTests
{
    [Fact]
    public void GenerateHash_ShouldProduceValidSaltAndPassword()
    {
        string rawPassword = "ClaveSegura123!";

        var result = PasswordHelper.GenerateHash(rawPassword);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Password));
        Assert.False(string.IsNullOrWhiteSpace(result.Salt));
        Assert.Equal(310000, result.Iterations);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        string rawPassword = "ClaveSegura123!";
        var hash = PasswordHelper.GenerateHash(rawPassword);

        bool isValid = PasswordHelper.VerifyPassword(rawPassword, hash.Salt!, hash.Password!);

        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        string rawPassword = "ClaveSegura123!";
        var hash = PasswordHelper.GenerateHash(rawPassword);

        bool isValid = PasswordHelper.VerifyPassword("ClaveEquivocada!", hash.Salt!, hash.Password!);

        Assert.False(isValid);
    }
}
