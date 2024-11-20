using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hermes.AppData.Migrations.Remote
{
    /// <inheritdoc />
    public partial class _002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DboUpdates",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DboUpdates", x => x.Name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Stop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRestored = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Details = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Actions = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClosedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stop", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Defect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StopId = table.Column<int>(type: "int", nullable: true),
                    UnitUnderTestId = table.Column<int>(type: "int", nullable: false),
                    ErrorFlag = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErrorCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Defect_Stop_StopId",
                        column: x => x.StopId,
                        principalTable: "Stop",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StopUser",
                columns: table => new
                {
                    StopsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopUser", x => new { x.StopsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_StopUser_Stop_StopsId",
                        column: x => x.StopsId,
                        principalTable: "Stop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StopUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Defect_StopId",
                table: "Defect",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_StopUser_UsersId",
                table: "StopUser",
                column: "UsersId");
            
            migrationBuilder.Sql("CREATE TRIGGER `update_dbupdates_on_user_insert` AFTER INSERT ON `users`\n FOR EACH ROW INSERT INTO dboupdates (Name, LastModified, Description)\n    VALUES ('users', NOW(), 'Last users table update or insert')\n    ON DUPLICATE KEY UPDATE LastModified = NOW()");
            migrationBuilder.Sql("CREATE TRIGGER `update_dbupdates_on_user_update` AFTER UPDATE ON `users`\n FOR EACH ROW INSERT INTO dboupdates (Name, LastModified, Description)\n    VALUES ('users', NOW(), 'Last users table update or insert')\n    ON DUPLICATE KEY UPDATE LastModified = NOW()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DboUpdates");

            migrationBuilder.DropTable(
                name: "Defect");

            migrationBuilder.DropTable(
                name: "StopUser");

            migrationBuilder.DropTable(
                name: "Stop");
            
            migrationBuilder.Sql("DROP TRIGGER `update_dbupdates_on_user_insert`");
            migrationBuilder.Sql("DROP TRIGGER `update_dbupdates_on_user_update`");
        }
    }
}
