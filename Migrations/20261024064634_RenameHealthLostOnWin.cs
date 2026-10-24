using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AUBE.Migrations
{
    /// <inheritdoc />
    public partial class RenameHealthLostOnWin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "T2TotalHealthLostOnWIn",
                table: "Reports",
                newName: "T2TotalHealthLostOnWin");

            migrationBuilder.RenameColumn(
                name: "T1TotalHealthLostOnWIn",
                table: "Reports",
                newName: "T1TotalHealthLostOnWin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "T2TotalHealthLostOnWin",
                table: "Reports",
                newName: "T2TotalHealthLostOnWIn");

            migrationBuilder.RenameColumn(
                name: "T1TotalHealthLostOnWin",
                table: "Reports",
                newName: "T1TotalHealthLostOnWIn");
        }
    }
}
