using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenTypeToEmailVerificationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenType",
                table: "EmailVerificationTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenType",
                table: "EmailVerificationTokens");
        }
    }
}
