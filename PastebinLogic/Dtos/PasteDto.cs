namespace PastebinLogic.Dtos;

public class PasteDto
{
    public Guid Id { get; set; }

    public required string Content { get; set; }

    public string? Name { get; set; } = null;
}
