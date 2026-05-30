using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAgendamentoUso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mar_agendamentos_uso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    EmbarcacaoId = table.Column<int>(type: "integer", nullable: true),
                    DataHoraUso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataHoraPreparacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MinutosAntecedencia = table.Column<int>(type: "integer", nullable: false),
                    TipoUso = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_agendamentos_uso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_agendamentos_uso_mar_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "mar_clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mar_agendamentos_uso_mar_embarcacoes_EmbarcacaoId",
                        column: x => x.EmbarcacaoId,
                        principalTable: "mar_embarcacoes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mar_agendamentos_uso_sis_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "sis_empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mar_agendamentos_uso_ClienteId",
                table: "mar_agendamentos_uso",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_agendamentos_uso_EmbarcacaoId",
                table: "mar_agendamentos_uso",
                column: "EmbarcacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_agendamentos_uso_EmpresaId",
                table: "mar_agendamentos_uso",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mar_agendamentos_uso");
        }
    }
}
