using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsumoPlano : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CobrancaExtra",
                table: "mar_agendamentos_uso",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "mar_agendamentos_uso",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanoContratoServicoId",
                table: "mar_agendamentos_uso",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cfg_consumo_plano",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    PlanoContratoServicoId = table.Column<int>(type: "integer", nullable: false),
                    Periodo = table.Column<string>(type: "text", nullable: false),
                    QuantidadeUsada = table.Column<int>(type: "integer", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_consumo_plano", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cfg_consumo_plano_cfg_plano_contrato_servicos_PlanoContrato~",
                        column: x => x.PlanoContratoServicoId,
                        principalTable: "cfg_plano_contrato_servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cfg_consumo_plano_mar_contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "mar_contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mar_agendamentos_uso_ContratoId",
                table: "mar_agendamentos_uso",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_agendamentos_uso_PlanoContratoServicoId",
                table: "mar_agendamentos_uso",
                column: "PlanoContratoServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_cfg_consumo_plano_ContratoId_PlanoContratoServicoId_Periodo",
                table: "cfg_consumo_plano",
                columns: new[] { "ContratoId", "PlanoContratoServicoId", "Periodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cfg_consumo_plano_PlanoContratoServicoId",
                table: "cfg_consumo_plano",
                column: "PlanoContratoServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_mar_agendamentos_uso_cfg_plano_contrato_servicos_PlanoContr~",
                table: "mar_agendamentos_uso",
                column: "PlanoContratoServicoId",
                principalTable: "cfg_plano_contrato_servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_mar_agendamentos_uso_mar_contratos_ContratoId",
                table: "mar_agendamentos_uso",
                column: "ContratoId",
                principalTable: "mar_contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mar_agendamentos_uso_cfg_plano_contrato_servicos_PlanoContr~",
                table: "mar_agendamentos_uso");

            migrationBuilder.DropForeignKey(
                name: "FK_mar_agendamentos_uso_mar_contratos_ContratoId",
                table: "mar_agendamentos_uso");

            migrationBuilder.DropTable(
                name: "cfg_consumo_plano");

            migrationBuilder.DropIndex(
                name: "IX_mar_agendamentos_uso_ContratoId",
                table: "mar_agendamentos_uso");

            migrationBuilder.DropIndex(
                name: "IX_mar_agendamentos_uso_PlanoContratoServicoId",
                table: "mar_agendamentos_uso");

            migrationBuilder.DropColumn(
                name: "CobrancaExtra",
                table: "mar_agendamentos_uso");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "mar_agendamentos_uso");

            migrationBuilder.DropColumn(
                name: "PlanoContratoServicoId",
                table: "mar_agendamentos_uso");
        }
    }
}
