using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHorasAluguelToPedidoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HorasAluguel",
                table: "mar_pedido_itens",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorasAluguel",
                table: "mar_pedido_itens");
        }
    }
}
