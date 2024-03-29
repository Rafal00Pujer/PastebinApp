﻿using Microsoft.EntityFrameworkCore;
using PastebinDatabase.EntityConfigurations;
using PastebinDatabase.EntityHelperTypes;
using System.Diagnostics.CodeAnalysis;

namespace PastebinDatabase.Entities;

[EntityTypeConfiguration(typeof(PasteMetaConfiguration))]
public class PasteMetaEntity
{
    public required Guid PasteId { get; set; }

    public required PasteVisibility Visibility { get; set; }

    public DateTime? ExpirationDate { get; set; } = null;

    public bool BurnOnRead { get; set; } = false;

    [MemberNotNullWhen(true, nameof(Password))]
    public bool PasswordProtected { get; set; } = false;

    public virtual PasteEntity Paste { get; set; } = null!;

    public virtual PastePasswordEntity? Password { get; set; } = null;
}