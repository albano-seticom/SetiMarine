using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddValorDuracaoTipoServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DuracaoHoras",
                table: "mar_tipos_servico_config",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPadrao",
                table: "mar_tipos_servico_config",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracaoHoras",
                table: "mar_tipos_servico_config");

            migrationBuilder.DropColumn(
                name: "ValorPadrao",
                table: "mar_tipos_servico_config");
        }
    }
}
