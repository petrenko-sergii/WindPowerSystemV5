using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindPowerSystemV5.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInfoFileUriColumnToTurbineTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InfoFileUri",
                table: "TurbineTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InfoFileUri",
                table: "TurbineTypes");
        }
    }
}
