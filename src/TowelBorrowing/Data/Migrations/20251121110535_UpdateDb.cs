using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowelBorrowing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "BorrowRecords",
                newName: "ReturnQuantity");

            migrationBuilder.AddColumn<int>(
                name: "BorrowQuantity",
                table: "BorrowRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowQuantity",
                table: "BorrowRecords");

            migrationBuilder.RenameColumn(
                name: "ReturnQuantity",
                table: "BorrowRecords",
                newName: "Quantity");
        }
    }
}
