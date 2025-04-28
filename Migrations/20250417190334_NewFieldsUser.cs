using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldsUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dienstnummer",
                table: "AspNetUsers",
                newName: "dienstnummer");

            migrationBuilder.RenameColumn(
                name: "Displayname",
                table: "AspNetUsers",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "DiscordId",
                table: "AspNetUsers",
                newName: "discord_id");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "AspNetUsers",
                newName: "avatar");

            migrationBuilder.AlterColumn<int>(
                name: "dienstnummer",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "besoldung",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "birthday",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "birthday_message_sent",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "erstellt_am",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fired_at",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "phone",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "prefix",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "rangs_id",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "teams_id",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "total_hours",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "besoldung",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "birthday",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "birthday_message_sent",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "erstellt_am",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "fired_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "prefix",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "rangs_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "status",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "teams_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "total_hours",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "dienstnummer",
                table: "AspNetUsers",
                newName: "Dienstnummer");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "AspNetUsers",
                newName: "Displayname");

            migrationBuilder.RenameColumn(
                name: "discord_id",
                table: "AspNetUsers",
                newName: "DiscordId");

            migrationBuilder.RenameColumn(
                name: "avatar",
                table: "AspNetUsers",
                newName: "AvatarUrl");

            migrationBuilder.AlterColumn<int>(
                name: "Dienstnummer",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
