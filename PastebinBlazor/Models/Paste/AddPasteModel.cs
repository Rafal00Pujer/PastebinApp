using PastebinDatabase.EntityHelperTypes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PastebinBlazor.Models.Paste;

public class AddPasteModel : IValidatableObject
{
    private PasteVisibility visibility;
    private bool passwordProtected;

    [MaxLength(25, ErrorMessage = "Name cannot exeed 25 characters.")]
    public string? Name { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Content { get; set; } = string.Empty;

    public DateTime? ExpirationDate { get; set; }

    public PasteVisibility Visibility
    {
        get => visibility;
        set
        {
            visibility = value;

            if (visibility == PasteVisibility.Public)
            {
                BurnOnRead = false;
                PasswordProtected = false;
            }
        }
    }

    public bool BurnOnRead { get; set; }

    [MemberNotNullWhen(true, nameof(Password))]
    public bool PasswordProtected
    {
        get => passwordProtected;
        set
        {
            passwordProtected = value;

            if (!passwordProtected)
            {
                Password = null;
            }
        }
    }

    [MinLength(6, ErrorMessage = "Password must have atleast 6 characters.")]
    public string? Password { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        const int MinimumNumOfMinutes = 5;

        if (ExpirationDate is not null && ExpirationDate < DateTime.UtcNow.AddMinutes(-MinimumNumOfMinutes))
        {
            yield return new ValidationResult($"Expiration date must be minimum {MinimumNumOfMinutes} minutes in the future.",
             new[] { nameof(ExpirationDate) });
        }

        if (Visibility == PasteVisibility.Public)
        {
            if (BurnOnRead)
            {
                yield return new ValidationResult($"Burn on read must be false when visibility is set to public.",
                    new[] { nameof(BurnOnRead) });
            }

            if (PasswordProtected)
            {
                yield return new ValidationResult($"Password protected must be false when visibility is set to public.",
                    new[] { nameof(BurnOnRead) });
            }
        }

        if (PasswordProtected && Password is null)
        {
            yield return new ValidationResult($"Password must have value when password protected is set to true.",
                    new[] { nameof(Password) });
        }
    }
}
