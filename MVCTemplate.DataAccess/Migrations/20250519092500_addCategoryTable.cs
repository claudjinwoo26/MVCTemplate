using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCTemplate.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeCategory",
                table: "Categorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categorys",
                keyColumn: "IdCategory",
                keyValue: 1,
                columns: new[] { "CodeCategory", "CreatedAt" },
                values: new object[] { "1C", new DateTime(2025, 5, 19, 17, 25, 0, 3, DateTimeKind.Local).AddTicks(4402) });

            migrationBuilder.UpdateData(
                table: "Categorys",
                keyColumn: "IdCategory",
                keyValue: 2,
                columns: new[] { "CodeCategory", "CreatedAt" },
                values: new object[] { "2C", new DateTime(2025, 5, 19, 17, 25, 0, 3, DateTimeKind.Local).AddTicks(4412) });

            migrationBuilder.UpdateData(
                table: "Categorys",
                keyColumn: "IdCategory",
                keyValue: 3,
                columns: new[] { "CodeCategory", "CreatedAt" },
                values: new object[] { "3C", new DateTime(2025, 5, 19, 17, 25, 0, 3, DateTimeKind.Local).AddTicks(4413) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeCategory",
                table: "Categorys");

            migrationBuilder.UpdateData(
                table: "Categorys",
                keyColumn: "IdCategory",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 19, 16, 59, 0, 359, DateTimeKind.Local).AddTicks(3520));

            migrationBuilder.UpdateData(
                table: "Categorys",
                keyColumn: "IdCategory",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 19, 16, 59, 0, 359, DateTimeKind.Local).AddTicks(3531));

            migrationBuilder.UpdateData(
                table: "Categorys",
                keyColumn: "IdCategory",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 19, 16, 59, 0, 359, DateTimeKind.Local).AddTicks(3532));
        }
    }
}
