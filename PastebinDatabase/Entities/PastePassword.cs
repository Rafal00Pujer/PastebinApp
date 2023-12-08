namespace PastebinDatabase.Entities;

public class PastePassword
{
    public required Guid PasteId { get; set; }

    public required string PasswordHash { get; set; }

    public required string PasswordSalt { get; set; }
}