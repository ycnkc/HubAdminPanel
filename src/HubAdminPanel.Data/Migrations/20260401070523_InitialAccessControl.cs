using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HubAdminPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialAccessControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EndpointPermissionMappings");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UserExtraPermissions");

            migrationBuilder.DropTable(
                name: "Endpoints");
        }
    }
}
