using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class MapDutyStatusAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Duties",
                type: "enum('IN_SERVICE','OUT_OF_SERVICE','PENDING','CANCELLED')",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Duties",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('IN_SERVICE','OUT_OF_SERVICE','PENDING','CANCELLED')")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
