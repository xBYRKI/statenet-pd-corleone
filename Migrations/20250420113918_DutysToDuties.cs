using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class DutysToDuties : Migration
    {
        /// <inheritdoc />
         protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Neue Duties-Tabelle (falls noch nicht vorhanden)
            migrationBuilder.CreateTable( 
                name: "Duties",
                columns: table => new
                {
                    Id           = table.Column<int>(nullable: false)
                                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordId    = table.Column<string>(maxLength: 255, nullable: false),
                    Status       = table.Column<string>(
                                        type: "enum('IN_SERVICE','OUT_OF_SERVICE','PENDING','CANCELLED')",
                                        nullable: false,
                                        defaultValue: "PENDING"),
                    LastCheck    = table.Column<DateTime>(nullable: true),
                    RightAnswer  = table.Column<int>(nullable: true),
                    StartTime    = table.Column<DateTime>(nullable: true),
                    EndTime      = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duties", x => x.Id);
                });

            // 2) Datenübernahme aus der alten Tabelle `dutys`
            migrationBuilder.Sql(@"
                INSERT INTO `Duties`
                    (`DiscordId`, `Status`, `LastCheck`, `RightAnswer`, `StartTime`, `EndTime`)
                SELECT
                    `discord_id`,
                    `status`,
                    `last_check`,
                    `rightAnswer`,
                    `start_time`,
                    `end_time`
                FROM `dutys`;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Lösche die neue Tabelle
            migrationBuilder.DropTable(name: "Duties");
        }
    }
}
