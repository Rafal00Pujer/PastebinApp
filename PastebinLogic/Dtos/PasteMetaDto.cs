namespace PastebinLogic.Dtos;

public class PasteMetaDto
{
    public Guid PasteId { get; set; }

    public PasteVisibility Visibility { get; set; }

    public DateTime? ExpirationDate { get; set; } = null;

    public bool BurnOnRead { get; set; } = false;

    public bool PasswordProtected { get; set; } = false;
}
