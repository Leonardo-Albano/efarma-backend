using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFarma.Migrations
{
    /// <inheritdoc />
    public partial class PrescriptionItemMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observation",
                table: "PrescriptionItems",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observation",
                table: "PrescriptionItems");
        }
    }
}
