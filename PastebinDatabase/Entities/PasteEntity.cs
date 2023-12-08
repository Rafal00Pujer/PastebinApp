namespace PastebinDatabase.Entities;

public class PasteEntity
{
    public Guid Id { get; set; }

    public required string Content { get; set; }

    public string? Name { get; set; }
}