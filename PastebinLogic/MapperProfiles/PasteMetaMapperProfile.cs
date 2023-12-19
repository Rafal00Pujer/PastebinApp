namespace PastebinLogic.MapperProfiles;

internal class PasteMetaMapperProfile : Profile
{
    public PasteMetaMapperProfile()
    {
        CreateMap<AddPasteDto, PasteMetaEntity>()
            .ForMember(e => e.PasteId,
                o => o.Ignore())
            .ForMember(e => e.Visibility,
                o => o.Ignore())
            .ForMember(e => e.Paste,
                o => o.Ignore())
            .ForMember(e => e.Password,
                o => o.Ignore());

        CreateMap<PasteMetaEntity, PasteMetaDto>();
    }
}