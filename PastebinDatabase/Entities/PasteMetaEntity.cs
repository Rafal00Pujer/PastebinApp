using PastebinDatabase.EntityHelperTypes;

namespace PastebinDatabase.Entities;

public class PasteMetaEntity
{
    //public Guid Id { get; set; }

    public required Guid PasteId { get; set; }

    public required PasteVisibility Visibility { get; set; }

    public DateTime? ExpirationDate { get; set; } = null;

    public bool BurnOnRead { get; set; } = false;

    public bool PasswordProtected { get; set; } = false;
}