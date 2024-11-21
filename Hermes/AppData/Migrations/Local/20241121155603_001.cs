using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hermes.AppData.Migrations.Local
{
    /// <inheritdoc />
    public partial class _001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "feature_permissions",
                columns: table => new
                {
                    Permission = table.Column<int>(type: "INTEGER", nullable: false),
                    Department = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feature_permissions", x => new { x.Permission, x.Department, x.Level });
                });

            migrationBuilder.CreateTable(
                name: "sfc_responses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    FullPath = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sfc_responses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRestored = table.Column<bool>(type: "INTEGER", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false),
                    Actions = table.Column<string>(type: "TEXT", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Department = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "units_under_test",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    IsFail = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StopId = table.Column<int>(type: "INTEGER", nullable: true),
                    SfcResponseId = table.Column<int>(type: "INTEGER", nullable: true),
                    FullPath = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units_under_test", x => x.Id);
                    table.ForeignKey(
                        name: "FK_units_under_test_sfc_responses_SfcResponseId",
                        column: x => x.SfcResponseId,
                        principalTable: "sfc_responses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_units_under_test_stops_StopId",
                        column: x => x.StopId,
                        principalTable: "stops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user_stop",
                columns: table => new
                {
                    StopsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stop", x => new { x.StopsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_user_stop_stops_StopsId",
                        column: x => x.StopsId,
                        principalTable: "stops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_stop_users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "defects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StopId = table.Column<int>(type: "INTEGER", nullable: true),
                    UnitUnderTestId = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorFlag = table.Column<int>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ErrorCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_defects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_defects_stops_StopId",
                        column: x => x.StopId,
                        principalTable: "stops",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_defects_units_under_test_UnitUnderTestId",
                        column: x => x.UnitUnderTestId,
                        principalTable: "units_under_test",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_defects_StopId",
                table: "defects",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_defects_UnitUnderTestId",
                table: "defects",
                column: "UnitUnderTestId");

            migrationBuilder.CreateIndex(
                name: "IX_units_under_test_SfcResponseId",
                table: "units_under_test",
                column: "SfcResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_units_under_test_StopId",
                table: "units_under_test",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_user_stop_UsersId",
                table: "user_stop",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "defects");

            migrationBuilder.DropTable(
                name: "feature_permissions");

            migrationBuilder.DropTable(
                name: "user_stop");

            migrationBuilder.DropTable(
                name: "units_under_test");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "sfc_responses");

            migrationBuilder.DropTable(
                name: "stops");
        }
    }
}
