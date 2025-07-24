using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdWP.Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_Projects_ProjectId",
                table: "Links");

            migrationBuilder.DropForeignKey(
                name: "FK_Links_Valuations_ValuationId",
                table: "Links");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "UserGroupTypes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CostTypes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CostCategories",
                newName: "id");

            migrationBuilder.AlterColumn<Guid>(
                name: "ValuationId",
                table: "Links",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Links",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Links",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Projects_ProjectId",
                table: "Links",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Valuations_ValuationId",
                table: "Links",
                column: "ValuationId",
                principalTable: "Valuations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_Projects_ProjectId",
                table: "Links");

            migrationBuilder.DropForeignKey(
                name: "FK_Links_Valuations_ValuationId",
                table: "Links");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserGroupTypes",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CostTypes",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CostCategories",
                newName: "ID");

            migrationBuilder.AlterColumn<Guid>(
                name: "ValuationId",
                table: "Links",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Links",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Links",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Projects_ProjectId",
                table: "Links",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Valuations_ValuationId",
                table: "Links",
                column: "ValuationId",
                principalTable: "Valuations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
