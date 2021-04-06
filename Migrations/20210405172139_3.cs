using Microsoft.EntityFrameworkCore.Migrations;

namespace FreshAir.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c15510a6-0ee8-4dc5-867e-2f2702516ed4");

            migrationBuilder.CreateTable(
                name: "ChatHistories",
                columns: table => new
                {
                    ChatHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistories", x => x.ChatHistoryId);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "51712ea3-596e-4842-a65d-09d147d1b225", "3b046971-e702-4caf-a717-db1856bcb483", "Athlete", "ATHLETE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistories");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51712ea3-596e-4842-a65d-09d147d1b225");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c15510a6-0ee8-4dc5-867e-2f2702516ed4", "31ba488f-aa4e-44b9-ac38-479557400523", "Athlete", "ATHLETE" });
        }
    }
}
