using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desa.Core.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "public",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Pedidos",
                schema: "public",
                newName: "Pedidos");

            migrationBuilder.RenameTable(
                name: "Motos",
                schema: "public",
                newName: "Motos");

            migrationBuilder.RenameTable(
                name: "Entregadores",
                schema: "public",
                newName: "Entregadores");

            migrationBuilder.AddColumn<int>(
                name: "IdEntregador",
                table: "Pedidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Locacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdMoto = table.Column<int>(type: "integer", nullable: false),
                    IdEntregador = table.Column<int>(type: "integer", nullable: false),
                    DiasLocacao = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataTermino = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataPrevisaoTermino = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValorLocacao = table.Column<decimal>(type: "numeric", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locacoes");

            migrationBuilder.DropColumn(
                name: "IdEntregador",
                table: "Pedidos");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Pedidos",
                newName: "Pedidos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Motos",
                newName: "Motos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Entregadores",
                newName: "Entregadores",
                newSchema: "public");
        }
    }
}
