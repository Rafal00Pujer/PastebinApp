namespace PastebinDatabase.Entities;

public class PasteEntity
{
    public Guid Id { get; set; }

    public required string Content { get; set; }

    public string? Name { get; set; }

    public virtual PasteMetaEntity Meta { get; set; } = null;

    public virtual PastePasswordEntity? Password { get; set; }
}