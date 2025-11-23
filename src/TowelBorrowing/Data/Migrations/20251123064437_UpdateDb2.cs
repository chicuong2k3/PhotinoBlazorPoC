using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowelBorrowing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BorrowRecords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BorrowRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
