using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVenteStatusAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAnnulation",
                table: "Ventes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RaisonAnnulation",
                table: "Ventes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Ventes",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Normal")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Ventes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_UserId",
                table: "Ventes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventes_Users_UserId",
                table: "Ventes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventes_Users_UserId",
                table: "Ventes");

            migrationBuilder.DropIndex(
                name: "IX_Ventes_UserId",
                table: "Ventes");

            migrationBuilder.DropColumn(
                name: "DateAnnulation",
                table: "Ventes");

            migrationBuilder.DropColumn(
                name: "RaisonAnnulation",
                table: "Ventes");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Ventes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Ventes");
        }
    }
}
