namespace PastebinLogic.MapperProfiles;

internal class PasteMapperProfile : Profile
{
    public PasteMapperProfile()
    {
        CreateMap<AddPasteDto, PasteEntity>()
            .ForMember(e => e.Id,
                o => o.Ignore())
            .ForMember(e => e.Meta,
                o => o.Ignore());
    }
}