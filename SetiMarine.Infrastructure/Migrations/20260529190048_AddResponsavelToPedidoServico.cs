using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsavelToPedidoServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponsavelId",
                table: "mar_pedido_servicos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mar_pedido_servicos_ResponsavelId",
                table: "mar_pedido_servicos",
                column: "ResponsavelId");

            migrationBuilder.AddForeignKey(
                name: "FK_mar_pedido_servicos_sis_usuarios_ResponsavelId",
                table: "mar_pedido_servicos",
                column: "ResponsavelId",
                principalTable: "sis_usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mar_pedido_servicos_sis_usuarios_ResponsavelId",
                table: "mar_pedido_servicos");

            migrationBuilder.DropIndex(
                name: "IX_mar_pedido_servicos_ResponsavelId",
                table: "mar_pedido_servicos");

            migrationBuilder.DropColumn(
                name: "ResponsavelId",
                table: "mar_pedido_servicos");
        }
    }
}
