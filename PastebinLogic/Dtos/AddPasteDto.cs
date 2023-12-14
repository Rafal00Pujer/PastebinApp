using System.Diagnostics.CodeAnalysis;
using PastebinDatabase.EntityHelperTypes;

namespace PastebinLogic.Dtos;

public class AddPasteDto
{
    public required string Content { get; set; }

    public string? Name { get; set; } = null;

    public required PasteVisibility Visibility { get; set; }

    public DateTime? ExpirationDate { get; set; } = null;

    public bool BurnOnRead { get; set; } = false;

    [MemberNotNullWhen(true, nameof(Password))]
    public bool PasswordProtected { get; set; } = false;

    public string? Password { get; set; } = null;
}