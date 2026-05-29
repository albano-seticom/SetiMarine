using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecoAluguelHoraToProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoAluguelHora",
                table: "mar_produtos",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoAluguelHora",
                table: "mar_produtos");
        }
    }
}
