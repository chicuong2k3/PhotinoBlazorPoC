using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowelBorrowing.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuestCards",
                columns: table => new
                {
                    CardNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    HolderName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Building = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Floor = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    RoomNo = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestCards", x => x.CardNumber);
                });

            migrationBuilder.CreateTable(
                name: "BorrowRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuestCardNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowRecords_GuestCards_GuestCardNumber",
                        column: x => x.GuestCardNumber,
                        principalTable: "GuestCards",
                        principalColumn: "CardNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_GuestCardNumber",
                table: "BorrowRecords",
                column: "GuestCardNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowRecords");

            migrationBuilder.DropTable(
                name: "GuestCards");
        }
    }
}
