using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitUsersNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserUnit_AspNetUsers_UserId",
                table: "UserUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUnit_Units_UnitId",
                table: "UserUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserUnit",
                table: "UserUnit");

            migrationBuilder.DropIndex(
                name: "IX_UserUnit_UserId",
                table: "UserUnit");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "HRActions");

            migrationBuilder.DropColumn(
                name: "NewRank",
                table: "HRActions");

            migrationBuilder.DropColumn(
                name: "NewSalary",
                table: "HRActions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "HRActions");

            migrationBuilder.DropColumn(
                name: "OldRank",
                table: "HRActions");

            migrationBuilder.DropColumn(
                name: "OldSalary",
                table: "HRActions");

            migrationBuilder.RenameTable(
                name: "UserUnit",
                newName: "UserUnits");

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Units",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Permission",
                table: "RolePermissions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "HRActions",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "HRActions",
                type: "enum('Hire','Termination','Sanction','Promotion','Demotion','Suspension')",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HRActions",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "HRActions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserUnits",
                table: "UserUnits",
                columns: new[] { "UserId", "UnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserUnits_UnitId",
                table: "UserUnits",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserUnits_AspNetUsers_UserId",
                table: "UserUnits",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUnits_Units_UnitId",
                table: "UserUnits",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserUnits_AspNetUsers_UserId",
                table: "UserUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUnits_Units_UnitId",
                table: "UserUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserUnits",
                table: "UserUnits");

            migrationBuilder.DropIndex(
                name: "IX_UserUnits_UnitId",
                table: "UserUnits");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "HRActions");

            migrationBuilder.RenameTable(
                name: "UserUnits",
                newName: "UserUnit");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Units",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Permission",
                table: "RolePermissions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "HRActions",
                keyColumn: "Reason",
                keyValue: null,
                column: "Reason",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "HRActions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ActionType",
                table: "HRActions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('Hire','Termination','Sanction','Promotion','Demotion','Suspension')")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "HRActions",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "HRActions",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewRank",
                table: "HRActions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "NewSalary",
                table: "HRActions",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "HRActions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OldRank",
                table: "HRActions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "OldSalary",
                table: "HRActions",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserUnit",
                table: "UserUnit",
                columns: new[] { "UnitId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserUnit_UserId",
                table: "UserUnit",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserUnit_AspNetUsers_UserId",
                table: "UserUnit",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUnit_Units_UnitId",
                table: "UserUnit",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
