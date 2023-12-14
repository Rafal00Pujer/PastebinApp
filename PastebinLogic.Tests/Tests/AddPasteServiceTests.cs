namespace PastebinLogic.Tests.Tests;

[Collection(TestsConfigs.UsesDatabase)]
public class AddPasteServiceTests : IClassFixture<DatabaseContextFixture>, IClassFixture<MapperFixture>, IDisposable
{
    private readonly AddPasteService _sut;
    private readonly PastebinContext _context;
    private readonly Mock<IPasswordService> _passwordServiceMoq;
    private readonly IFixture _fixture;

    public AddPasteServiceTests(DatabaseContextFixture databaseContextFixture, MapperFixture mapperFixture)
    {
        var mapper = mapperFixture.Fixture;

        var fixture = new Fixture();

        var context = databaseContextFixture.Fixture;
        context.Database.EnsureCreated();

        _passwordServiceMoq = new Mock<IPasswordService>();
        _sut = new AddPasteService(context, mapper, _passwordServiceMoq.Object);
        _context = context;
        _fixture = fixture;
    }

    #region AddPasteAsyncValidCases
    [Fact]
    public async Task AddPasteAsync_DtoWithoutPasswordAndExpirationDate_CreatesValidPasteWithoudPasswordAndExpirationDate()
    {
        // Arrange
        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .Create();

        // Act
        var result = await _sut.AddPasteAsync(dto);

        // Assert
        var pasteEntity = await _context.Pastes
            .Include(x => x.Meta)
                .ThenInclude(x => x.Password)
            .FirstAsync();

        pasteEntity.Should().NotBeNull();

        result.Id.Should().Be(pasteEntity.Id);

        pasteEntity.Content.Should().Be(dto.Content);
        pasteEntity.Name.Should().Be(dto.Name);

        var metaEntity = pasteEntity.Meta;

        metaEntity.Should().NotBeNull();
        metaEntity.Visibility.Should().Be(dto.Visibility);
        metaEntity.BurnOnRead.Should().Be(dto.BurnOnRead);
        metaEntity.PasswordProtected.Should().Be(dto.PasswordProtected);

        metaEntity.ExpirationDate.Should().BeNull();
        metaEntity.Password.Should().BeNull();
    }

    [Fact]
    public async Task AddPasteAsync_DtoWithoutPasswordAndWithValidExpirationDate_CreatesValidPasteWithoudPasswordAndValidExpirationDate()
    {
        // Arrange
        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.ExpirationDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .Create();

        // Act
        var result = await _sut.AddPasteAsync(dto);

        // Assert
        var pasteEntity = await _context.Pastes
            .Include(x => x.Meta)
                .ThenInclude(x => x.Password)
            .FirstAsync();

        pasteEntity.Should().NotBeNull();

        result.Id.Should().Be(pasteEntity.Id);

        pasteEntity.Content.Should().Be(dto.Content);
        pasteEntity.Name.Should().Be(dto.Name);

        var metaEntity = pasteEntity.Meta;

        metaEntity.Should().NotBeNull();
        metaEntity.Visibility.Should().Be(dto.Visibility);
        metaEntity.BurnOnRead.Should().Be(dto.BurnOnRead);
        metaEntity.PasswordProtected.Should().Be(dto.PasswordProtected);
        metaEntity.ExpirationDate.Should().Be(dto.ExpirationDate);

        metaEntity.Password.Should().BeNull();
    }

    [Fact]
    public async Task AddPasteAsync_DtoWithPasswordAndWithoutExpirationDate_CreatesValidPasteWithPasswordAndWithoutExpirationDate()
    {
        // Arrange
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        _passwordServiceMoq.Setup(x => x.GeneratePasswordSaltAndHash(password))
            .Returns((salt, hash));

        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, true)
            .With(x => x.Password, password)
            .Create();

        // Act
        var result = await _sut.AddPasteAsync(dto);

        // Assert
        var pasteEntity = await _context.Pastes
            .Include(x => x.Meta)
                .ThenInclude(x => x.Password)
            .FirstAsync();

        pasteEntity.Should().NotBeNull();

        result.Id.Should().Be(pasteEntity.Id);

        pasteEntity.Content.Should().Be(dto.Content);
        pasteEntity.Name.Should().Be(dto.Name);

        var metaEntity = pasteEntity.Meta;

        metaEntity.Should().NotBeNull();
        metaEntity.Visibility.Should().Be(dto.Visibility);
        metaEntity.BurnOnRead.Should().Be(dto.BurnOnRead);
        metaEntity.PasswordProtected.Should().Be(dto.PasswordProtected);

        metaEntity.ExpirationDate.Should().BeNull();

        var passwordEntity = metaEntity.Password;

        passwordEntity.Should().NotBeNull();
        passwordEntity!.PasswordHash.Should().Be(hash);
        passwordEntity!.PasswordSalt.Should().Be(salt);
    }

    [Fact]
    public async Task AddPasteAsync_DtoWithPasswordAndValidExpirationDate_CreatesValidPasteWithPasswordAndValidExpirationDate()
    {
        // Arrange
        var salt = _fixture.Create<string>();
        var hash = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        _passwordServiceMoq.Setup(x => x.GeneratePasswordSaltAndHash(password))
            .Returns((salt, hash));

        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.ExpirationDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.PasswordProtected, true)
            .With(x => x.Password, password)
            .Create();

        // Act
        var result = await _sut.AddPasteAsync(dto);

        // Assert
        var pasteEntity = await _context.Pastes
            .Include(x => x.Meta)
                .ThenInclude(x => x.Password)
            .FirstAsync();

        pasteEntity.Should().NotBeNull();

        result.Id.Should().Be(pasteEntity.Id);

        pasteEntity.Content.Should().Be(dto.Content);
        pasteEntity.Name.Should().Be(dto.Name);

        var metaEntity = pasteEntity.Meta;

        metaEntity.Should().NotBeNull();
        metaEntity.Visibility.Should().Be(dto.Visibility);
        metaEntity.BurnOnRead.Should().Be(dto.BurnOnRead);
        metaEntity.PasswordProtected.Should().Be(dto.PasswordProtected);
        metaEntity.ExpirationDate.Should().Be(dto.ExpirationDate);

        var passwordEntity = metaEntity.Password;

        passwordEntity.Should().NotBeNull();
        passwordEntity!.PasswordHash.Should().Be(hash);
        passwordEntity!.PasswordSalt.Should().Be(salt);
    }
    #endregion

    #region AddPasteAsyncExceptionCases
    [Fact]
    public async Task AddPasteAsync_DtoWithoutContent_ThrowsException()
    {
        // Arrange
        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.Content, () => null!)
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .Create();

        // Act
        var action = async () => await _sut.AddPasteAsync(dto);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
             .WithMessage($"{nameof(dto.Content)} is null.*");
    }

    [Fact]
    public async Task AddPasteAsync_DtoWithPasswordProtectionAndNoPassword_ThrowsException()
    {
        // Arrange
        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, true)
            .With(x => x.Password, () => null)
            .Create();

        // Act
        var action = async () => await _sut.AddPasteAsync(dto);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
             .WithMessage($"Paste is {nameof(dto.PasswordProtected)}, but {nameof(dto.Password)} is null.*");
    }

    [Fact]
    public async Task AddPasteAsync_DtoWithNoPasswordProtectionAndPassword_ThrowsException()
    {
        // Arrange
        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.ExpirationDate, () => null)
            .With(x => x.PasswordProtected, false)
            .Create();

        // Act
        var action = async () => await _sut.AddPasteAsync(dto);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
             .WithMessage($"Paste is not {nameof(dto.PasswordProtected)}, but {nameof(dto.Password)} is not null.*");
    }

    [Fact]
    public async Task AddPasteAsync_DtoWithExpirationDateInPast_ThrowsException()
    {
        // Arrange
        var dto = _fixture.Build<AddPasteDto>()
            .With(x => x.Name, (string y) => y[..25])
            .With(x => x.ExpirationDate, DateTime.UtcNow.AddDays(-1))
            .With(x => x.PasswordProtected, false)
            .With(x => x.Password, () => null)
            .Create();

        // Act
        var action = async () => await _sut.AddPasteAsync(dto);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
             .WithMessage($"{nameof(dto.ExpirationDate)} is in past.*");
    } 
    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
