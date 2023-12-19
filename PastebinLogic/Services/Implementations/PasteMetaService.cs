using Microsoft.EntityFrameworkCore;

namespace PastebinLogic.Services.Implementations;

internal class PasteMetaService(PastebinContext context, IMapper mapper) : IPasteMetaService
{
    private readonly PastebinContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<PasteMetaDto>> GetAllPasteMetaAsync()
    {
        var pasteMetas = _mapper.ProjectTo<PasteMetaDto>(_context.PastesMetas);

        return await pasteMetas.ToListAsync();
    }

    public async Task<PasteMetaDto> GetPasteMetaAsync(Guid pasteId)
    {
        var pasteMeta = await _mapper.ProjectTo<PasteMetaDto>(_context.PastesMetas)
                                .FirstOrDefaultAsync(pm => pm.PasteId == pasteId);

        return pasteMeta is null
            ? throw new InvalidOperationException($"Paste meta with id:{pasteId} doesn't exist.")
            : pasteMeta;
    }
}