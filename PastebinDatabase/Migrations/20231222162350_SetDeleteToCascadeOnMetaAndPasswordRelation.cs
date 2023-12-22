using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PastebinDatabase.Migrations
{
    /// <inheritdoc />
    public partial class SetDeleteToCascadeOnMetaAndPasswordRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pastes_Passwords_Pastes_Metas_PasteId",
                table: "Pastes_Passwords");

            migrationBuilder.AddForeignKey(
                name: "FK_Pastes_Passwords_Pastes_Metas_PasteId",
                table: "Pastes_Passwords",
                column: "PasteId",
                principalTable: "Pastes_Metas",
                principalColumn: "PasteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pastes_Passwords_Pastes_Metas_PasteId",
                table: "Pastes_Passwords");

            migrationBuilder.AddForeignKey(
                name: "FK_Pastes_Passwords_Pastes_Metas_PasteId",
                table: "Pastes_Passwords",
                column: "PasteId",
                principalTable: "Pastes_Metas",
                principalColumn: "PasteId");
        }
    }
}
