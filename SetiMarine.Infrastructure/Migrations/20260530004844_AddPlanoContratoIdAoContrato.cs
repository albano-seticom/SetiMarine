using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanoContratoIdAoContrato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanoContratoId",
                table: "mar_contratos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mar_contratos_PlanoContratoId",
                table: "mar_contratos",
                column: "PlanoContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_mar_contratos_cfg_planos_contrato_PlanoContratoId",
                table: "mar_contratos",
                column: "PlanoContratoId",
                principalTable: "cfg_planos_contrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mar_contratos_cfg_planos_contrato_PlanoContratoId",
                table: "mar_contratos");

            migrationBuilder.DropIndex(
                name: "IX_mar_contratos_PlanoContratoId",
                table: "mar_contratos");

            migrationBuilder.DropColumn(
                name: "PlanoContratoId",
                table: "mar_contratos");
        }
    }
}
