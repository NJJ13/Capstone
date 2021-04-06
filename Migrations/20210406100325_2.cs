using Microsoft.EntityFrameworkCore.Migrations;

namespace FreshAir.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b7833fa-d8f2-46a7-adfa-265a99125ec5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "25b1a802-1699-4f3d-97cc-e9ef69fc200a", "af115b8d-c8be-4772-806f-5fd9bba5d719", "Athlete", "ATHLETE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25b1a802-1699-4f3d-97cc-e9ef69fc200a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6b7833fa-d8f2-46a7-adfa-265a99125ec5", "684a8a19-526f-4e1b-8c9d-170bc56ce253", "Athlete", "ATHLETE" });
        }
    }
}
