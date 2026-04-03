using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HubAdminPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddControllerNameToEndpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ControllerName",
                table: "Endpoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControllerName",
                table: "Endpoints");
        }
    }
}
