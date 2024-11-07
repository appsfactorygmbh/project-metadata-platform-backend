using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceProjectLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Changes",
                table: "Logs");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Logs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LogChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: false),
                    NewValue = table.Column<string>(type: "text", nullable: false),
                    Property = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogChange_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogChange_LogId",
                table: "LogChange",
                column: "LogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs");

            migrationBuilder.DropTable(
                name: "LogChange");

            migrationBuilder.DropIndex(
                name: "IX_Logs_UserId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Logs");

            migrationBuilder.AddColumn<string>(
                name: "Changes",
                table: "Logs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
