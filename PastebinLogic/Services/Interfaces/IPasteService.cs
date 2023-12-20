namespace PastebinLogic.Services.Interfaces;

public interface IPasteService
{
    public Task<IEnumerable<PasteDto>> GetAllPublicPasteAsync();

    public Task<PasteDto> GetPasteAsync(Guid pasteId, string? password = null);
}