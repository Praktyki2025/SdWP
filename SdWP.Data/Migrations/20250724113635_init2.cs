using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdWP.Data.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_AspNetUsers_UserId",
                table: "ErrorLogs");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetUsers_Name",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ErrorLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_AspNetUsers_UserId",
                table: "ErrorLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_AspNetUsers_UserId",
                table: "ErrorLogs");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ErrorLogs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetUsers_Name",
                table: "AspNetUsers",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_AspNetUsers_UserId",
                table: "ErrorLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
