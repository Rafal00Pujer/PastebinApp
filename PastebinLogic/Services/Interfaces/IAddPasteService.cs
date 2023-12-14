namespace PastebinLogic.Services.Interfaces;

public interface IAddPasteService
{
    public Task<AddPasteResultDto> AddPasteAsync(AddPasteDto dto);
}