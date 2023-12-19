namespace PastebinLogic.Services.Interfaces;

public interface IPasteMetaService
{
    public Task<IEnumerable<PasteMetaDto>> GetAllPasteMetaAsync();

    public Task<PasteMetaDto> GetPasteMetaAsync(Guid pasteId);
}