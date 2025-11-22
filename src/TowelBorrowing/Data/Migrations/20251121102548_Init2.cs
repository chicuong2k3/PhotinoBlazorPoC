using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowelBorrowing.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaxClientMonitors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Building = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    RoomNo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    MaxQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaxClientMonitors", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaxClientMonitors");
        }
    }
}
