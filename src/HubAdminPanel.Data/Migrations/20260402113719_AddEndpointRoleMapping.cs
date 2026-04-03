using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HubAdminPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEndpointRoleMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "EndpointRoleMappings",
                columns: table => new
                {
                    EndpointId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointRoleMappings", x => new { x.EndpointId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_EndpointRoleMappings_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EndpointRoleMappings_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndpointRoleMappings_RoleId",
                table: "EndpointRoleMappings",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EndpointRoleMappings");

            migrationBuilder.CreateTable(
                name: "EndpointPermissionMappings",
                columns: table => new
                {
                    EndpointId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointPermissionMappings", x => new { x.EndpointId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_EndpointPermissionMappings_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EndpointPermissionMappings_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EndpointUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndpointId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndpointUsers_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EndpointUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExtraPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndpointId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExtraPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExtraPermissions_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExtraPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndpointPermissionMappings_PermissionId",
                table: "EndpointPermissionMappings",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_EndpointUsers_EndpointId",
                table: "EndpointUsers",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_EndpointUsers_UserId",
                table: "EndpointUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExtraPermissions_EndpointId",
                table: "UserExtraPermissions",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExtraPermissions_PermissionId",
                table: "UserExtraPermissions",
                column: "PermissionId");
        }
    }
}
