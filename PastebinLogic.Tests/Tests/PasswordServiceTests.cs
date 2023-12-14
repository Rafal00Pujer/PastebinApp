using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace PastebinLogic.Tests.Tests;

public class PasswordServiceTests
{
    private readonly PasswordService _sut = new();
    private readonly IFixture _fixture = new Fixture();

    #region GeneratePasswordSaltAndHash
    [Fact]
    public void GeneratePasswordSaltAndHash_ReturnsValidSaltNadHash()
    {
        // Arrange
        var password = _fixture.Create<string>();

        // Act
        var result = _sut.GeneratePasswordSaltAndHash(password);

        // Assert
        result.salt.Should().NotBeNullOrWhiteSpace();
        result.hash.Should().NotBeNullOrWhiteSpace();

        Convert.ToBase64String(KeyDerivation.Pbkdf2(password,
            Convert.FromBase64String(result.salt),
            PasswordService.Prf,
            PasswordService.IterationCount,
            PasswordService.NumBytesHash))
            .Should().Be(result.hash);
    }

    [Fact]
    public void GeneratePasswordSaltAndHash_ThrowsWhenPasswordIsNull()
    {
        // Arrange

        // Act
        var action = () => _sut.GeneratePasswordSaltAndHash(null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void GeneratePasswordSaltAndHash_ThrowsWhenPasswordIsEmpty()
    {
        // Arrange

        // Act
        var action = () => _sut.GeneratePasswordSaltAndHash(string.Empty);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void GeneratePasswordSaltAndHash_ThrowsWhenPasswordIsWhiteSpace()
    {
        // Arrange

        // Act
        var action = () => _sut.GeneratePasswordSaltAndHash(" ");

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }
    #endregion

    #region PasswordIsValid
    [Fact]
    public void PasswordIsValid_ReturnsTrueForValidPassword()
    {
        // Arrange
        var password = _fixture.Create<string>();

        var saltInBytes = RandomNumberGenerator.GetBytes(PasswordService.NumBytesSalt);

        var salt = Convert.ToBase64String(saltInBytes);
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(password,
            saltInBytes,
            PasswordService.Prf,
            PasswordService.IterationCount,
            PasswordService.NumBytesHash));

        // Act
        var result = _sut.PasswordIsValid(salt, hash, password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void PasswordIsValid_ReturnsFalseForInValidPassword()
    {
        // Arrange
        var validPassword = _fixture.Create<string>();
        var inValidPassword = _fixture.Create<string>();

        var saltInBytes = RandomNumberGenerator.GetBytes(PasswordService.NumBytesSalt);

        var salt = Convert.ToBase64String(saltInBytes);
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(validPassword,
            saltInBytes,
            PasswordService.Prf,
            PasswordService.IterationCount,
            PasswordService.NumBytesHash));

        // Act
        var result = _sut.PasswordIsValid(salt, hash, inValidPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenPasswordIsNull()
    {
        // Arrange
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(salt, hash, null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenPasswordIsEmpty()
    {
        // Arrange
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(salt, hash, string.Empty);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenPasswordIsWhiteSpace()
    {
        // Arrange
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(salt, hash, " ");

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenSaltIsNull()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(null!, hash, password);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenSaltIsEmpty()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(string.Empty, hash, password);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenSaltIsWhiteSpace()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(" ", hash, password);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenHashIsNull()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var salt = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(salt, null!, password);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenHashIsEmpty()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var salt = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(salt, string.Empty, password);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void PasswordIsValid_ThrowsWhenHashIsWhiteSpace()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var salt = _fixture.Create<string>();

        // Act
        var action = () => _sut.PasswordIsValid(salt, " ", password);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }
    #endregion
}
