using PastebinLogic.MapperProfiles;

namespace PastebinLogic.Tests.Fixtures;

public class MapperFixture
{
    public IMapper Fixture { get; private set; }

    public MapperFixture()
    {
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(PasteMapperProfile).Assembly));
        Fixture = config.CreateMapper();
    }
}
