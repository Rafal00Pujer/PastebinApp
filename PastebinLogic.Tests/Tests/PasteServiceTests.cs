using PastebinDatabase.Entities;

namespace PastebinLogic.Tests.Tests;

[Collection(TestsConfigs.UsesDatabase)]
public class PasteServiceTests : IClassFixture<DatabaseContextFixture>, IClassFixture<MapperFixture>, IDisposable
{
    private readonly PasteService _sut;
    private readonly PastebinContext _context;
    private readonly Mock<IPasswordService> _passwordServiceMoq;
    private readonly IFixture _fixture;

    public PasteServiceTests(DatabaseContextFixture databaseContextFixture, MapperFixture mapperFixture)
    {
        var mapper = mapperFixture.Fixture;

        var fixture = new Fixture();

        var context = databaseContextFixture.Fixture;
        context.Database.EnsureCreated();

        _passwordServiceMoq = new Mock<IPasswordService>();
        _sut = new PasteService(context, mapper, _passwordServiceMoq.Object);
        _context = context;
        _fixture = fixture;
    }

    #region GetPasteAsync
    [Fact]
    public async Task GetPasteAsync_ValidIdWithoutPassword_ReturnsUnprotectedPaste()
    {
        // Arrange
        var pasteId = Guid.NewGuid();

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetPasteAsync(pasteId);

        // Assert
        result.Id.Should().Be(pasteId);
        result.Name.Should().Be(paste.Name);
        result.Content.Should().Be(paste.Content);
    }

    [Fact]
    public async Task GetPasteAsync_InvalidIdWithoutPassword_ThrowsException()
    {
        // Arrange
        var pasteId = Guid.NewGuid();
        var methodId = Guid.NewGuid();

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var action = async () => await _sut.GetPasteAsync(methodId);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Paste with id:{methodId} doesn't exist.");
    }

    [Fact]
    public async Task GetPasteAsync_ValidIdWithoutPasswordForExpiredPaste_DeletesPasteAndThrowsException()
    {
        // Arrange
        var pasteId = Guid.NewGuid();

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, DateTime.UtcNow.AddSeconds(-1))
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var action = async () => await _sut.GetPasteAsync(pasteId);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Paste with id:{pasteId} expired and was deleted.");

        var pasteInDb = await _context.Pastes.FirstOrDefaultAsync(p => p.Id == pasteId);
        pasteInDb.Should().BeNull();
    }

    [Fact]
    public async Task GetPasteAsync_ValidIdWithoutPasswordAndBurnOnRead_ReturnsUnprotectedPasteAndDeletesPaste()
    {
        // Arrange
        var pasteId = Guid.NewGuid();

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.BurnOnRead, true)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetPasteAsync(pasteId);

        // Assert
        result.Id.Should().Be(pasteId);
        result.Name.Should().Be(paste.Name);
        result.Content.Should().Be(paste.Content);

        var pasteInDb = await _context.Pastes.FirstOrDefaultAsync(p => p.Id == pasteId);
        pasteInDb.Should().BeNull();
    }

    [Fact]
    public async Task GetPasteAsync_ValidIdForProtecedPasteWithPassword_ReturnsProtectedPaste()
    {
        // Arrange
        var pasteId = Guid.NewGuid();
        var password = _fixture.Create<string>();
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        var passwordEntity = new PastePasswordEntity
        {
            PasteId = pasteId,
            PasswordSalt = salt,
            PasswordHash = hash
        };

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, true)
            .With(x => x.Password, passwordEntity)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        _passwordServiceMoq.Setup(x => x.PasswordIsValid(salt, hash, password))
            .Returns(true);

        // Act
        var result = await _sut.GetPasteAsync(pasteId, password);

        // Assert
        result.Id.Should().Be(pasteId);
        result.Name.Should().Be(paste.Name);
        result.Content.Should().Be(paste.Content);
    }

    [Fact]
    public async Task GetPasteAsync_ValidIdForProtecedPasteWithoutPassword_ThrowsException()
    {
        // Arrange
        var pasteId = Guid.NewGuid();
        var password = _fixture.Create<string>();
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        var passwordEntity = new PastePasswordEntity
        {
            PasteId = pasteId,
            PasswordSalt = salt,
            PasswordHash = hash
        };

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, true)
            .With(x => x.Password, passwordEntity)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var action = async () => await _sut.GetPasteAsync(pasteId);

        // Assert
        await action.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage($"Paste with id:{pasteId} is protected, but password is null.");
    }

    [Fact]
    public async Task GetPasteAsync_ValidIdForProtecedPasteWithInvalidPassword_ThrowsException()
    {
        // Arrange
        var pasteId = Guid.NewGuid();
        var validPassword = _fixture.Create<string>();
        var invalidPassword = _fixture.Create<string>();
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();

        var passwordEntity = new PastePasswordEntity
        {
            PasteId = pasteId,
            PasswordSalt = salt,
            PasswordHash = hash
        };

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, true)
            .With(x => x.Password, passwordEntity)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        _passwordServiceMoq.Setup(x => x.PasswordIsValid(salt, hash, invalidPassword))
            .Returns(false);

        // Act
        var action = async () => await _sut.GetPasteAsync(pasteId, invalidPassword);

        // Assert
        await action.Should().ThrowExactlyAsync<UnauthorizedAccessException>()
            .WithMessage($"Provided password for paste with id:{pasteId} is invalid.");
    }
    #endregion

    [Fact]
    public async Task GetAllPublicPasteAsync_ReturnsPublicNotProtectedPastes()
    {
        // Arrange
        var pasteId = Guid.NewGuid();

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.Visibility, PasteVisibility.Public)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        pasteId = Guid.NewGuid();

        meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.Visibility, PasteVisibility.Link)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        pasteId = Guid.NewGuid();

        meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.Visibility, PasteVisibility.Public)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, true)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllPublicPasteAsync();

        // Assert
        result.Should().HaveCount(1);

        var pasteMetaInDb = await _context.PastesMetas.FirstAsync(x => x.PasteId == result.First().Id);
        pasteMetaInDb.Visibility.Should().Be(PasteVisibility.Public);
    }

    [Fact]
    public async Task GetAllPublicPasteAsync_ReturnsPublicNotProtectedPastesAndRemovesExpiredPastes()
    {
        // Arrange
        var pasteId = Guid.NewGuid();

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.Visibility, PasteVisibility.Public)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        pasteId = Guid.NewGuid();

        meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.Visibility, PasteVisibility.Public)
            .With(x => x.BurnOnRead, false)
            .With(x => x.ExpirationDate, DateTime.UtcNow.AddSeconds(-1))
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllPublicPasteAsync();

        // Assert
        result.Should().HaveCount(1);

        var pastesMetaInDb = await _context.PastesMetas.ToListAsync();
        pastesMetaInDb.Should().HaveCount(1);
        pastesMetaInDb.First().ExpirationDate.Should().BeNull();
    }

    [Fact]
    public async Task GetAllPublicPasteAsync_ReturnsPublicNotProtectedPastesAndRemovesBurnOnReadPastes()
    {
        // Arrange
        var pasteId = Guid.NewGuid();

        var meta = _fixture.Build<PasteMetaEntity>()
            .With(x => x.PasteId, pasteId)
            .With(x => x.Visibility, PasteVisibility.Public)
            .With(x => x.BurnOnRead, true)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .With(x => x.Paste, () => null!)
            .Create();

        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Id, pasteId)
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Meta, meta)
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllPublicPasteAsync();

        // Assert
        result.Should().HaveCount(1);

        var pastesMetaInDb = await _context.PastesMetas.ToListAsync();
        pastesMetaInDb.Should().HaveCount(0);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}