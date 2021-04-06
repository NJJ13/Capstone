using Microsoft.EntityFrameworkCore.Migrations;

namespace FreshAir.Migrations
{
    public partial class _4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51712ea3-596e-4842-a65d-09d147d1b225");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "df1be5f8-af98-48cf-95f1-9985d484ee00", "ddc2185a-7436-4a82-96cf-a1ba4a2d4afe", "Athlete", "ATHLETE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "df1be5f8-af98-48cf-95f1-9985d484ee00");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "51712ea3-596e-4842-a65d-09d147d1b225", "3b046971-e702-4caf-a717-db1856bcb483", "Athlete", "ATHLETE" });
        }
    }
}
