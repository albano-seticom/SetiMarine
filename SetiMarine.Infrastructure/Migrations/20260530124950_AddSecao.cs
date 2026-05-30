using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSecao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecaoId",
                table: "mar_corredores",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "mar_secoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_secoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_secoes_sis_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "sis_empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mar_corredores_SecaoId",
                table: "mar_corredores",
                column: "SecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_secoes_EmpresaId",
                table: "mar_secoes",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_mar_corredores_mar_secoes_SecaoId",
                table: "mar_corredores",
                column: "SecaoId",
                principalTable: "mar_secoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mar_corredores_mar_secoes_SecaoId",
                table: "mar_corredores");

            migrationBuilder.DropTable(
                name: "mar_secoes");

            migrationBuilder.DropIndex(
                name: "IX_mar_corredores_SecaoId",
                table: "mar_corredores");

            migrationBuilder.DropColumn(
                name: "SecaoId",
                table: "mar_corredores");
        }
    }
}
