using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItemAndMenuItemRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_AspNetRoles_RoleRequiredId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_RoleRequiredId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "RoleRequiredId",
                table: "MenuItems");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Url",
                keyValue: null,
                column: "Url",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "MenuItems",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Section",
                keyValue: null,
                column: "Section",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "MenuItems",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Icon",
                keyValue: null,
                column: "Icon",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "MenuItems",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuItemRoles",
                columns: table => new
                {
                    MenuItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemRoles", x => new { x.MenuItemId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_MenuItemRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemRoles_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemRoles_RoleId",
                table: "MenuItemRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemRoles");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "MenuItems",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "MenuItems",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "MenuItems",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "MenuItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "MenuItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RoleRequiredId",
                table: "MenuItems",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_RoleRequiredId",
                table: "MenuItems",
                column: "RoleRequiredId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_AspNetRoles_RoleRequiredId",
                table: "MenuItems",
                column: "RoleRequiredId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
