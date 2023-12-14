using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PastebinDatabase.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pastes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pastes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pastes_Metas",
                columns: table => new
                {
                    PasteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BurnOnRead = table.Column<bool>(type: "bit", nullable: false),
                    PasswordProtected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pastes_Metas", x => x.PasteId);
                    table.ForeignKey(
                        name: "FK_Pastes_Metas_Pastes_PasteId",
                        column: x => x.PasteId,
                        principalTable: "Pastes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pastes_Passwords",
                columns: table => new
                {
                    PasteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pastes_Passwords", x => x.PasteId);
                    table.ForeignKey(
                        name: "FK_Pastes_Passwords_Pastes_Metas_PasteId",
                        column: x => x.PasteId,
                        principalTable: "Pastes_Metas",
                        principalColumn: "PasteId");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pastes_Passwords");

            migrationBuilder.DropTable(
                name: "Pastes_Metas");

            migrationBuilder.DropTable(
                name: "Pastes");
        }
    }
}
