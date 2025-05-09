using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class RankUsedr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_rangs_id",
                table: "AspNetUsers",
                column: "rangs_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Ranks_rangs_id",
                table: "AspNetUsers",
                column: "rangs_id",
                principalTable: "Ranks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Ranks_rangs_id",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_rangs_id",
                table: "AspNetUsers");
        }
    }
}
