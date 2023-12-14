namespace PastebinLogic.Services.Implementations;

internal class AddPasteService(PastebinContext context, IMapper mapper, IPasswordService passwordService) : IAddPasteService
{
    private readonly PastebinContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IPasswordService _passwordService = passwordService;

    public async Task<AddPasteResultDto> AddPasteAsync(AddPasteDto dto)
    {
        ValidateDto(dto);

        var paste = _mapper.Map<AddPasteDto, PasteEntity>(dto);
        await _context.Pastes.AddAsync(paste);

        var meta = new PasteMetaEntity
        {
            PasteId = paste.Id,
            Visibility = dto.Visibility
        };

        _mapper.Map(dto, meta);
        await _context.PastesMetas.AddAsync(meta);

        if (dto.PasswordProtected)
        {
            (var salt, var hash) = _passwordService.GeneratePasswordSaltAndHash(dto.Password);

            var pastePassword = new PastePasswordEntity
            {
                PasteId = paste.Id,
                PasswordSalt = salt,
                PasswordHash = hash
            };

            await _context.PastesPasswords.AddAsync(pastePassword);
        }

        await _context.SaveChangesAsync();

        var result = new AddPasteResultDto
        {
            Id = paste.Id
        };

        return result;
    }

    private static void ValidateDto(AddPasteDto dto)
    {
        if (dto.Content is null)
        {
            throw new ArgumentException($"{nameof(dto.Content)} is null.", nameof(dto));
        }

        if (dto is { PasswordProtected: true, Password: null })
        {
            throw new ArgumentException($"Paste is {nameof(dto.PasswordProtected)}, but {nameof(dto.Password)} is null.",
                nameof(dto));
        }

        if (dto is { PasswordProtected: false, Password: not null })
        {
            throw new ArgumentException(
                $"Paste is not {nameof(dto.PasswordProtected)}, but {nameof(dto.Password)} is not null.", nameof(dto));
        }

        if (dto.ExpirationDate is not null && dto.ExpirationDate < DateTime.UtcNow)
        {
            throw new ArgumentException($"{nameof(dto.ExpirationDate)} is in past.", nameof(dto));
        }
    }
}