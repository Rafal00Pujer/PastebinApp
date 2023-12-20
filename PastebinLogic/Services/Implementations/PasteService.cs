using Microsoft.EntityFrameworkCore;

namespace PastebinLogic.Services.Implementations;

internal class PasteService(PastebinContext context, IMapper mapper, IPasswordService passwordService) : IPasteService
{
    private readonly PastebinContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IPasswordService _passwordService = passwordService;

    public async Task<PasteDto> GetPasteAsync(Guid pasteId, string? password = null)
    {
        var paste = await _context.Pastes
                            .Include(p => p.Meta)
                                .ThenInclude(pm => pm.Password)
                            .FirstOrDefaultAsync(p => p.Id == pasteId)
                            ?? throw new InvalidOperationException($"Paste with id:{pasteId} doesn't exist.");

        var meta = paste.Meta;

        if (meta.ExpirationDate < DateTime.UtcNow)
        {
            _context.Pastes.Remove(paste);
            await _context.SaveChangesAsync();

            throw new InvalidOperationException($"Paste with id:{pasteId} expired and was deleted.");
        }

        if (meta.BurnOnRead)
        {
            _context.Pastes.Remove(paste);
            await _context.SaveChangesAsync();
        }

        if (meta.PasswordProtected == true)
        {
            if (password is null)
            {
                throw new InvalidOperationException($"Paste with id:{pasteId} is protected, but password is null.");
            }

            var salt = meta.Password.PasswordSalt;
            var hash = meta.Password.PasswordHash;

            if (!_passwordService.PasswordIsValid(salt, hash, password))
            {
                throw new UnauthorizedAccessException($"Provided password for paste with id:{pasteId} is invalid.");
            }
        }

        var dto = _mapper.Map<PasteEntity, PasteDto>(paste);

        return dto;
    }

    public async Task<IEnumerable<PasteDto>> GetAllPublicPasteAsync()
    {
        var pastes = await _context.Pastes
                .Include(p => p.Meta)
                .Where(p => p.Meta.Visibility == PasteVisibility.Public
                        && p.Meta.PasswordProtected == false)
                .ToListAsync();

        pastes = await RemoveExpiredPastesAsync(pastes);

        await BurnPastes(pastes);

        var dtos = _mapper.Map<IEnumerable<PasteEntity>, List<PasteDto>>(pastes)
                        .ToList();

        return dtos;
    }

    private async Task<List<PasteEntity>> RemoveExpiredPastesAsync(IEnumerable<PasteEntity> pastes)
    {
        var expiredPastes = pastes.Where(p => p.Meta.ExpirationDate < DateTime.UtcNow)
                                .ToList();

        _context.Pastes.RemoveRange(expiredPastes);
        await _context.SaveChangesAsync();

        return pastes.Except(expiredPastes)
                    .ToList();
    }

    public async Task BurnPastes(IEnumerable<PasteEntity> pastes)
    {
        var pastesToBurn = pastes.Where(p => p.Meta.BurnOnRead == true)
                                .ToList();

        _context.Pastes.RemoveRange(pastesToBurn);
        await _context.SaveChangesAsync();
    }
}