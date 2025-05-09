using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class CustomUserRoleMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_HRActions_AspNetUsers_UserId",
                table: "HRActions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUnits_AspNetUsers_UserId",
                table: "UserUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUnits_Units_UnitId",
                table: "UserUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Units",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserUnits",
                table: "UserUnits");

            migrationBuilder.DropIndex(
                name: "IX_UserUnits_UnitId",
                table: "UserUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HRActions",
                table: "HRActions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "units");

            migrationBuilder.RenameTable(
                name: "UserUnits",
                newName: "UserUnit");

            migrationBuilder.RenameTable(
                name: "HRActions",
                newName: "hr_actions");

            migrationBuilder.RenameIndex(
                name: "IX_HRActions_UserId",
                table: "hr_actions",
                newName: "IX_hr_actions_UserId");

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
                table: "AspNetUserRoles",
                keyColumn: "ApplicationUserId",
                keyValue: null,
                column: "ApplicationUserId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUserRoles",
                keyColumn: "ApplicationRoleId",
                keyValue: null,
                column: "ApplicationRoleId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationRoleId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationRoleId1",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ActionType",
                table: "hr_actions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('Hire','Termination','Sanction','Promotion','Demotion','Suspension')")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_units",
                table: "units",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "ApplicationUserId", "ApplicationRoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserUnit",
                table: "UserUnit",
                columns: new[] { "UnitId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_hr_actions",
                table: "hr_actions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationRoleId1",
                table: "AspNetUserRoles",
                column: "ApplicationRoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId1",
                table: "AspNetUserRoles",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserUnit_UserId",
                table: "UserUnit",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserRoles",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId1",
                table: "AspNetUserRoles",
                column: "ApplicationRoleId1",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId1",
                table: "AspNetUserRoles",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_hr_actions_AspNetUsers_UserId",
                table: "hr_actions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUnit_AspNetUsers_UserId",
                table: "UserUnit",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUnit_units_UnitId",
                table: "UserUnit",
                column: "UnitId",
                principalTable: "units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId1",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId1",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_hr_actions_AspNetUsers_UserId",
                table: "hr_actions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUnit_AspNetUsers_UserId",
                table: "UserUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUnit_units_UnitId",
                table: "UserUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_units",
                table: "units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationRoleId1",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId1",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserUnit",
                table: "UserUnit");

            migrationBuilder.DropIndex(
                name: "IX_UserUnit_UserId",
                table: "UserUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hr_actions",
                table: "hr_actions");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId1",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "units",
                newName: "Units");

            migrationBuilder.RenameTable(
                name: "UserUnit",
                newName: "UserUnits");

            migrationBuilder.RenameTable(
                name: "hr_actions",
                newName: "HRActions");

            migrationBuilder.RenameIndex(
                name: "IX_hr_actions_UserId",
                table: "HRActions",
                newName: "IX_HRActions_UserId");

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
                name: "ApplicationRoleId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "HRActions",
                type: "enum('Hire','Termination','Sanction','Promotion','Demotion','Suspension')",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units",
                table: "Units",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserUnits",
                table: "UserUnits",
                columns: new[] { "UserId", "UnitId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_HRActions",
                table: "HRActions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUnits_UnitId",
                table: "UserUnits",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserRoles",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HRActions_AspNetUsers_UserId",
                table: "HRActions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
