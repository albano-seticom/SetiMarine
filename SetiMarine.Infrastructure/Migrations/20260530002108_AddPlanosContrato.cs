using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanosContrato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cfg_planos_contrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Mensalidade = table.Column<decimal>(type: "numeric", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_planos_contrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cfg_planos_contrato_sis_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "sis_empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cfg_plano_contrato_servicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanoContratoId = table.Column<int>(type: "integer", nullable: false),
                    TipoServicoConfigId = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_plano_contrato_servicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cfg_plano_contrato_servicos_cfg_planos_contrato_PlanoContra~",
                        column: x => x.PlanoContratoId,
                        principalTable: "cfg_planos_contrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cfg_plano_contrato_servicos_mar_tipos_servico_config_TipoSe~",
                        column: x => x.TipoServicoConfigId,
                        principalTable: "mar_tipos_servico_config",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cfg_plano_contrato_servicos_PlanoContratoId",
                table: "cfg_plano_contrato_servicos",
                column: "PlanoContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_cfg_plano_contrato_servicos_TipoServicoConfigId",
                table: "cfg_plano_contrato_servicos",
                column: "TipoServicoConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_cfg_planos_contrato_EmpresaId",
                table: "cfg_planos_contrato",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cfg_plano_contrato_servicos");

            migrationBuilder.DropTable(
                name: "cfg_planos_contrato");
        }
    }
}
