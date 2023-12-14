using Microsoft.EntityFrameworkCore;
using PastebinDatabase.Context;

namespace PastebinLogic.Tests.Fixtures;

public class DatabaseContextFixture : IDisposable
{
    private const string ConnectionString = "Server=localhost;Database=PastebinTests;Trusted_Connection=True;TrustServerCertificate=True;";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public PastebinContext Fixture { get; private set; }

    public DatabaseContextFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                Fixture = new PastebinContext(new DbContextOptionsBuilder<PastebinContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);

                Fixture.Database.EnsureDeleted();
                //Fixture.Database.EnsureCreated();

                _databaseInitialized = true;
            }
            else
            {
                throw new Exception("Multiple database initializations are not supported.");
            }
        }
    }

    public void Dispose()
    {
        Fixture.Database.EnsureDeleted();
        Fixture.Dispose();
        _databaseInitialized = false;
    }
}
