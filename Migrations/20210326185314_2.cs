using System;
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
                keyValue: "38aec406-0e4d-4634-9615-02d235c35686");

            migrationBuilder.AlterColumn<string>(
                name: "Picture",
                table: "Locations",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7a1c72fc-44ab-4e51-affc-d2db6d09bee0", "dd7b91d3-4632-40d9-a9bb-8b1161c601fa", "Athlete", "ATHLETE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a1c72fc-44ab-4e51-affc-d2db6d09bee0");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Picture",
                table: "Locations",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "38aec406-0e4d-4634-9615-02d235c35686", "2eb5c831-cfd9-4217-8384-e327788ab917", "Athlete", "ATHLETE" });
        }
    }
}
