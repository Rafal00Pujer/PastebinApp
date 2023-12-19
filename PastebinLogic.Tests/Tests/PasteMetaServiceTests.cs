using PastebinDatabase.Entities;

namespace PastebinLogic.Tests.Tests;

[Collection(TestsConfigs.UsesDatabase)]
public class PasteMetaServiceTests : IClassFixture<DatabaseContextFixture>, IClassFixture<MapperFixture>, IDisposable
{
    private readonly PasteMetaService _sut;
    private readonly PastebinContext _context;
    private readonly IFixture _fixture;

    public PasteMetaServiceTests(DatabaseContextFixture databaseContextFixture, MapperFixture mapperFixture)
    {
        var mapper = mapperFixture.Fixture;

        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var context = databaseContextFixture.Fixture;
        context.Database.EnsureCreated();
        _sut = new PasteMetaService(context, mapper);
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllPasteMetaAsync_ReturnsAllPasteMetasInDatabase()
    {
        // Arrange
        var pastes = _fixture.Build<PasteEntity>()
            .With(x => x.Name, (string y) => y[..25])
            .CreateMany();

        await _context.Pastes.AddRangeAsync(pastes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllPasteMetaAsync();

        // Assert
        result.Should().HaveCount(pastes.Count());

        result.Select(x => x.PasteId)
            .Should()
            .Contain(pastes
                .Select(p => p.Meta.PasteId));

        result.Select(x => x.Visibility)
            .Should()
            .Contain(pastes
                .Select(p => p.Meta.Visibility));

        result.Select(x => x.ExpirationDate)
            .Should()
            .Contain(pastes
                .Select(p => p.Meta.ExpirationDate));

        result.Select(x => x.BurnOnRead)
            .Should()
            .Contain(pastes
                .Select(p => p.Meta.BurnOnRead));

        result.Select(x => x.PasswordProtected)
            .Should()
            .Contain(pastes
                .Select(p => p.Meta.PasswordProtected));
    }

    [Fact]
    public async Task GetPasteMetaAsync_ValidPasteId_ReturnsMeta()
    {
        // Arrange
        var paste = _fixture.Build<PasteEntity>()
            .With(x => x.Name, (string y) => y[..25])
            .Create();

        await _context.Pastes.AddAsync(paste);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetPasteMetaAsync(paste.Id);

        // Assert
        result.PasteId.Should().Be(paste.Id);
        result.Visibility.Should().Be(paste.Meta.Visibility);
        result.ExpirationDate.Should().Be(paste.Meta.ExpirationDate);
        result.BurnOnRead.Should().Be(paste.Meta.BurnOnRead);
        result.PasswordProtected.Should().Be(paste.Meta.PasswordProtected);
    }

    [Fact]
    public async Task GetPasteMetaAsync_InvalidPasteId_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var action = async () => await _sut.GetPasteMetaAsync(id);

        // Assert
        await action.Should().ThrowExactlyAsync<InvalidOperationException>()
             .WithMessage($"Paste meta with id:{id} doesn't exist.");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
