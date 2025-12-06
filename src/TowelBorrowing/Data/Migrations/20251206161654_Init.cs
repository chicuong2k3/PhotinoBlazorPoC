using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                name: "AppSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "GuestCards",
                columns: table => new
                {
                    CardNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HolderName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Building = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Floor = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RoomNo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestCards", x => x.CardNumber);
                });

            migrationBuilder.CreateTable(
                name: "MaxClientMonitors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Building = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RoomNo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MaxQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaxClientMonitors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BorrowRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuestCardNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BorrowQuantity = table.Column<int>(type: "integer", nullable: false),
                    ReturnQuantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "BorrowRecords");

            migrationBuilder.DropTable(
                name: "MaxClientMonitors");

            migrationBuilder.DropTable(
                name: "GuestCards");
        }
    }
}
