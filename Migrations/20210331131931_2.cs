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
                keyValue: "816c7c0c-856b-4e01-a611-ea11685e4d6a");

            migrationBuilder.CreateTable(
                name: "FriendsLists",
                columns: table => new
                {
                    CurrentUserId = table.Column<int>(nullable: false),
                    FriendId = table.Column<int>(nullable: false),
                    CurrentAthleteAthleteId = table.Column<int>(nullable: true),
                    FriendAthleteAthleteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendsLists", x => new { x.CurrentUserId, x.FriendId });
                    table.ForeignKey(
                        name: "FK_FriendsLists_Athletes_CurrentAthleteAthleteId",
                        column: x => x.CurrentAthleteAthleteId,
                        principalTable: "Athletes",
                        principalColumn: "AthleteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendsLists_Athletes_FriendAthleteAthleteId",
                        column: x => x.FriendAthleteAthleteId,
                        principalTable: "Athletes",
                        principalColumn: "AthleteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0ce5a87e-26ed-4385-a9b8-f0b2c5396bf7", "03dd8b0c-110d-4c38-9c9d-cd3adb1603fe", "Athlete", "ATHLETE" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendsLists_CurrentAthleteAthleteId",
                table: "FriendsLists",
                column: "CurrentAthleteAthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendsLists_FriendAthleteAthleteId",
                table: "FriendsLists",
                column: "FriendAthleteAthleteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendsLists");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ce5a87e-26ed-4385-a9b8-f0b2c5396bf7");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "816c7c0c-856b-4e01-a611-ea11685e4d6a", "da4c5551-4b15-4b1b-bf4f-cf63d5869c71", "Athlete", "ATHLETE" });
        }
    }
}
