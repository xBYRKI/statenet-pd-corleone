using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statenet_lspd.Migrations
{
    /// <inheritdoc />
    public partial class MigrateOldUsersIntoAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        INSERT INTO `AspNetUsers`
            (`Id`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`,
             `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`,
             `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`,
             `discord_id`, `display_name`, `avatar`, `prefix`,
             `dienstnummer`, `besoldung`, `erstellt_am`, `status`,
             `rangs_id`, `teams_id`, `phone`, `fired_at`, `birthday`,
             `birthday_message_sent`, `total_hours`)
        SELECT
            UUID(),                      -- neue GUID
            old.`password_hash`,
            UUID(),                      -- neuer SecurityStamp
            UUID(),                      -- neuer ConcurrencyStamp
            old.`phone`,
            1,
            0,
            NULL,
            1,
            0,
            old.`discord_id`,
            old.`display_name`,
            old.`avatar`,
            old.`prefix`,
            old.`dienstnummer`,
            old.`besoldung`,
            old.`erstellt_am`,
            old.`status`,
            old.`rangs_id`,
            old.`teams_id`,
            old.`phone`,
            old.`fired_at`,
            old.`birthday`,
            old.`birthday_message_sent`,
            old.`total_hours`
        FROM `AlteUserTabelle` AS old
        WHERE NOT EXISTS (
            SELECT 1 FROM `AspNetUsers` nu WHERE nu.`Id` = old.`Id`
        );
    ");
}


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE Id IN (SELECT Id FROM AlteUserTabelle);");
        }
    }
}
