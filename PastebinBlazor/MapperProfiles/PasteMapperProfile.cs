using AutoMapper;

namespace PastebinBlazor.MapperProfiles;

public class PasteMapperProfile : Profile
{
    public PasteMapperProfile()
    {
        CreateMap<AddPasteModel, AddPasteDto>();
    }
}
