using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindPowerSystemV5.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameFileNameColumnInTurbineTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InfoFileUri",
                table: "TurbineTypes",
                newName: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "TurbineTypes",
                newName: "InfoFileUri");
        }
    }
}
